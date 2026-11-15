using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoMoreTears.Modules.Requests.Applications.Services.Abstractions;
using NoMoreTears.Modules.Requests.Applications.Services.Configuration;
using NoMoreTears.Modules.Requests.Applications.Services.Extensions;
using NoMoreTears.Modules.Requests.Applications.Services.Files;
using NoMoreTears.Modules.Requests.Applications.Services.Mappings;
using NoMoreTears.Modules.Requests.Applications.Services.Results;
using NoMoreTears.Modules.Requests.Contracts.Responses;
using NoMoreTears.Modules.Requests.Domain;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Modules.Requests.Domain.Exceptions;
using NoMoreTears.Modules.Requests.Domain.ValueObjects;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Domain;
using NoMoreTears.Shared.FileStorage.Abstractions;
using NoMoreTears.Shared.FileStorage.Exceptions;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Requests.Applications.Services;

/// <summary>
///     Asynchronous acceptance of photos (staging pattern).
/// </summary>
public sealed class MediaUploadService(
    IRepository<MediaUpload, Guid> uploads,
    IRepository<Request, Guid> requests,
    IRepository<MasterAccess, Guid> masterAccesses,
    IFileStorage fileStorage,
    IOptions<RequestsStorageOptions> storageOptions,
    TimeProvider timeProvider,
    ILogger<MediaUploadService> logger) : IMediaUploadService
{
    private readonly RequestsStorageOptions _storage = storageOptions.Value;

    /// <inheritdoc />
    public async Task<StagedMediaUpload> StageAsync(
        Guid clientId,
        Guid requestId,
        Stream content,
        string fileName,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(content);

        var request = await requests.GetOwnedForReadingAsync(clientId, requestId, cancellationToken);

        // Early feedback; the rules are checked again by the domain when the photo is attached.
        if (request.Status is not RequestStatus.Draft)
        {
            throw new RequestContentLockedException(request.Status);
        }

        var pendingCount = await uploads.CountAsync(
            upload => upload.RequestId == requestId && upload.Status == UploadStatus.Pending,
            cancellationToken);

        if (request.MediaFiles.Count + pendingCount >= RequestRules.MaxMediaFiles)
        {
            throw new MediaFileLimitExceededException(RequestRules.MaxMediaFiles);
        }

        var uploadId = Guid.CreateVersion7();
        var upload = MediaUpload.Create(
            uploadId,
            requestId,
            clientId,
            fileName,
            contentType,
            sizeInBytes,
            string.Create(CultureInfo.InvariantCulture, $"{requestId:N}/{uploadId:N}"),
            timeProvider.GetUtcNow());

        await fileStorage.UploadFileAsync(content, _storage.StagingBucket, upload.StagingKey, upload.ContentType, cancellationToken);
        await uploads.CreateAsync(upload, cancellationToken);

        logger.LogInformation("Upload {UploadId} for request {RequestId} staged for the check", uploadId, requestId);
        return new StagedMediaUpload(upload.ToResponse(), clientId);
    }

    /// <inheritdoc />
    public async Task<MediaUploadResponse> GetAsync(
        Guid clientId,
        Guid requestId,
        Guid uploadId,
        CancellationToken cancellationToken)
    {
        var upload = await uploads
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                found => found.Id == uploadId && found.RequestId == requestId && found.ClientId == clientId,
                cancellationToken)
            ?? throw new NotFoundException(string.Format(CultureInfo.InvariantCulture, "Upload '{0}' was not found.", uploadId));

        return upload.ToResponse();
    }

    /// <inheritdoc />
    public async Task<MediaUploadProcessingResult?> ProcessAsync(
        Guid uploadId,
        CancellationToken cancellationToken)
    {
        var upload = await uploads
            .AsQueryable()
            .FirstOrDefaultAsync(found => found.Id == uploadId, cancellationToken);

        // Redelivered message: the upload has already been finished.
        if (upload is null || upload.Status is not UploadStatus.Pending)
        {
            return null;
        }

        var stagingFile = new StoredFileResponse(_storage.StagingBucket, upload.StagingKey);
        var now = timeProvider.GetUtcNow();

        await using var content = await TryDownloadStagedAsync(upload, cancellationToken);

        var rejection = content is null
            ? "The file is no longer available, upload it again."
            : Check(upload, content);

        if (rejection is not null)
        {
            return await FailAsync(upload, rejection, now, stagingFile, cancellationToken);
        }

        // A discarded draft is hidden by the soft delete filter.
        var request = await requests.FindTrackedAsync(upload.RequestId, cancellationToken);

        if (request is null)
        {
            return await FailAsync(upload, "The request is no longer available.", now, stagingFile, cancellationToken);
        }

        var key = string.Create(
            CultureInfo.InvariantCulture,
            $"{upload.RequestId:N}/{upload.Id:N}{FileSignatures.ExtensionFor(upload.ContentType)}");
        Guid mediaFileId;

        try
        {
            mediaFileId = request.AttachMediaFile(upload.ClientId, key, upload.ContentType, upload.SizeInBytes, now).Id;
        }
        catch (DomainException ex)
        {
            // For example, the request was published or reached the photo limit while the photo waited for the check.
            return await FailAsync(upload, ex.Message, now, stagingFile, cancellationToken);
        }

        content!.Position = 0;
        await fileStorage.UploadFileAsync(content, _storage.MediaBucket, key, upload.ContentType, cancellationToken);

        upload.Complete(mediaFileId, now);
        await requests.UpdateAsync(request, cancellationToken);
        await uploads.UpdateAsync(upload, cancellationToken);

        logger.LogInformation("Upload {UploadId} for request {RequestId} accepted", upload.Id, upload.RequestId);
        return new MediaUploadProcessingResult(upload.ToResponse(), upload.ClientId, request.ToResponse(), [stagingFile]);
    }

    /// <inheritdoc />
    public async Task<FileContentResponse> OpenForClientAsync(
        Guid clientId,
        Guid requestId,
        Guid mediaFileId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetOwnedForReadingAsync(clientId, requestId, cancellationToken);
        return await OpenAsync(request, mediaFileId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<FileContentResponse> OpenForMasterAsync(
        Guid masterId,
        Guid requestId,
        Guid mediaFileId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetForReadingAsync(requestId, cancellationToken);

        if (!await masterAccesses.IsVisibleToMasterAsync(request, masterId, timeProvider.GetUtcNow(), cancellationToken))
        {
            throw RepositoryExtensions.RequestNotFound(requestId);
        }

        return await OpenAsync(request, mediaFileId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<FileContentResponse> OpenForSupportAsync(
        Guid requestId,
        Guid mediaFileId,
        CancellationToken cancellationToken)
    {
        var request = await requests.GetForReadingAsync(requestId, cancellationToken);
        return await OpenAsync(request, mediaFileId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteFilesAsync(
        IReadOnlyCollection<StoredFileResponse> files,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(files);

        foreach (var file in files)
        {
            await fileStorage.DeleteFileAsync(file.Bucket, file.Key, cancellationToken);
        }

        logger.LogInformation("{Count} obsolete files removed", files.Count);
    }

    private async Task<FileContentResponse> OpenAsync(
        Request request,
        Guid mediaFileId,
        CancellationToken cancellationToken)
    {
        var mediaFile = request.MediaFiles.FirstOrDefault(attached => attached.Id == mediaFileId)
            ?? throw new NotFoundException(string.Format(CultureInfo.InvariantCulture, "Media file '{0}' was not found.", mediaFileId));

        MemoryStream content;

        try
        {
            content = await fileStorage.DownloadFileAsync(_storage.MediaBucket, mediaFile.StorageKey, cancellationToken);
        }
        catch (FileNotFoundInStorageException)
        {
            throw new NotFoundException("The file was not found in the storage.");
        }

        return new FileContentResponse(
            content,
            mediaFile.ContentType,
            string.Create(CultureInfo.InvariantCulture, $"photo-{mediaFile.Id:N}{FileSignatures.ExtensionFor(mediaFile.ContentType)}"));
    }

    private async Task<MemoryStream?> TryDownloadStagedAsync(
        MediaUpload upload,
        CancellationToken cancellationToken)
    {
        try
        {
            return await fileStorage.DownloadFileAsync(_storage.StagingBucket, upload.StagingKey, cancellationToken);
        }
        catch (FileNotFoundInStorageException)
        {
            return null;
        }
    }

    private static string? Check(
        MediaUpload upload,
        MemoryStream content)
    {
        if (content.Length != upload.SizeInBytes)
        {
            return "The size of the file does not match the declared size.";
        }

        return FileSignatures.Matches(content.GetBuffer().AsSpan(0, (int)content.Length), upload.ContentType)
            ? null
            : "The content of the file does not match its type.";
    }

    private async Task<MediaUploadProcessingResult> FailAsync(
        MediaUpload upload,
        string reason,
        DateTimeOffset now,
        StoredFileResponse stagingFile,
        CancellationToken cancellationToken)
    {
        upload.Fail(Reason.Of(reason), now);
        await uploads.UpdateAsync(upload, cancellationToken);

        logger.LogInformation("Upload {UploadId} for request {RequestId} rejected", upload.Id, upload.RequestId);
        return new MediaUploadProcessingResult(upload.ToResponse(), upload.ClientId, null, [stagingFile]);
    }
}