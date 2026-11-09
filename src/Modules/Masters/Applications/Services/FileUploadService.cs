using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoMoreTears.Modules.Masters.Applications.Services.Abstractions;
using NoMoreTears.Modules.Masters.Applications.Services.Configuration;
using NoMoreTears.Modules.Masters.Applications.Services.Extensions;
using NoMoreTears.Modules.Masters.Applications.Services.Files;
using NoMoreTears.Modules.Masters.Applications.Services.Mappings;
using NoMoreTears.Modules.Masters.Applications.Services.Results;
using NoMoreTears.Modules.Masters.Contracts.Responses;
using NoMoreTears.Modules.Masters.Domain.Aggregates;
using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Modules.Masters.Domain.Exceptions;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.Domain;
using NoMoreTears.Shared.FileStorage.Abstractions;
using NoMoreTears.Shared.FileStorage.Exceptions;
using NoMoreTears.Shared.Postgres.Abstractions.Repositories;

namespace NoMoreTears.Modules.Masters.Applications.Services;

/// <summary>
///     Asynchronous acceptance of files (staging pattern):
///     <list type="number">
///         <item>the API stores the raw file in the temporary bucket, registers a pending upload and answers at once;</item>
///         <item>a consumer checks the content (signature, size), copies it to the permanent private bucket and attaches it;</item>
///         <item>files that are no longer referenced are removed by another consumer after the change is committed.</item>
///     </list>
/// </summary>
public sealed class FileUploadService(
    IRepository<FileUpload, Guid> uploads,
    IRepository<Master, Guid> masters,
    IFileStorage fileStorage,
    IOptions<MastersStorageOptions> storageOptions,
    TimeProvider timeProvider,
    ILogger<FileUploadService> logger) : IFileUploadService
{
    private const string StoredFileName = "file";

    private readonly MastersStorageOptions _storage = storageOptions.Value;

    /// <inheritdoc />
    public Task<StagedUpload> StageAvatarAsync(
        Guid masterId,
        Stream content,
        string fileName,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken) =>
        StageAsync(masterId, UploadPurpose.Avatar, null, content, fileName, contentType, sizeInBytes, cancellationToken);

    /// <inheritdoc />
    public Task<StagedUpload> StageDocumentAsync(
        Guid masterId,
        string kind,
        Guid? categoryId,
        Stream content,
        string fileName,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<DocumentKind>(kind, ignoreCase: true, out var documentKind))
        {
            throw new BadRequestException($"Document kind '{kind}' is unknown.");
        }

        var purpose = documentKind is DocumentKind.Identity
            ? UploadPurpose.IdentityDocument
            : UploadPurpose.QualificationDocument;

        return StageAsync(masterId, purpose, categoryId, content, fileName, contentType, sizeInBytes, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<FileUploadResponse> GetAsync(
        Guid masterId,
        Guid uploadId,
        CancellationToken cancellationToken)
    {
        var upload = await uploads
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(found => found.Id == uploadId && found.MasterId == masterId, cancellationToken)
            ?? throw new NotFoundException(string.Format(CultureInfo.InvariantCulture, "Upload '{0}' was not found.", uploadId));

        return upload.ToResponse();
    }

    /// <inheritdoc />
    public async Task<UploadProcessingResult?> ProcessAsync(
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

        var master = await masters.GetRequiredAsync(upload.MasterId, cancellationToken);
        var (bucket, key) = PermanentLocationOf(upload);
        var obsoleteFiles = new List<StoredFileResponse> { stagingFile };
        Guid? documentId;

        try
        {
            documentId = Attach(master, upload, key, now, obsoleteFiles);
        }
        catch (DomainException ex)
        {
            // For example, the category was removed from the profile while the file waited for the check.
            return await FailAsync(upload, ex.Message, now, stagingFile, cancellationToken);
        }

        content!.Position = 0;
        await fileStorage.UploadFileAsync(content, bucket, key, upload.ContentType, cancellationToken);

        upload.Complete(documentId, now);
        await masters.UpdateAsync(master, cancellationToken);
        await uploads.UpdateAsync(upload, cancellationToken);

        logger.LogInformation("Upload {UploadId} of master {MasterId} accepted", upload.Id, upload.MasterId);
        return new UploadProcessingResult(upload.ToResponse(), upload.MasterId, master.ToResponse(now), obsoleteFiles);
    }

    /// <inheritdoc />
    public async Task<FileContentResponse> OpenAvatarAsync(
        Guid masterId,
        CancellationToken cancellationToken)
    {
        var avatar = await masters
            .AsQueryable()
            .AsNoTracking()
            .Where(master => master.Id == masterId)
            .Select(master => master.Avatar)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("The profile has no avatar.");

        var content = await DownloadAsync(_storage.AvatarsBucket, avatar.StorageKey, cancellationToken);
        return new FileContentResponse(content, avatar.ContentType, $"avatar{FileSignatures.ExtensionFor(avatar.ContentType)}");
    }

    /// <inheritdoc />
    public async Task<FileContentResponse> OpenDocumentAsync(
        Guid masterId,
        Guid documentId,
        CancellationToken cancellationToken)
    {
        var master = await masters
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(profile => profile.Id == masterId, cancellationToken)
            ?? throw new NotFoundException(string.Format(CultureInfo.InvariantCulture, "Master profile '{0}' was not found.", masterId));

        var document = master.Documents.FirstOrDefault(uploaded => uploaded.Id == documentId)
            ?? throw new NotFoundException(string.Format(CultureInfo.InvariantCulture, "Document '{0}' was not found.", documentId));

        var content = await DownloadAsync(_storage.DocumentsBucket, document.StorageKey, cancellationToken);

        return new FileContentResponse(
            content,
            document.ContentType,
            $"{document.Kind.ToString().ToLowerInvariant()}-{document.Id:N}{FileSignatures.ExtensionFor(document.ContentType)}");
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

    private async Task<StagedUpload> StageAsync(
        Guid masterId,
        UploadPurpose purpose,
        Guid? categoryId,
        Stream content,
        string fileName,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(content);

        var master = await masters.GetRequiredAsync(masterId, cancellationToken);

        // Early feedback; the rule is checked again by the domain when the file is attached.
        if (categoryId is { } chosenCategoryId && master.Categories.All(category => category.CategoryId != chosenCategoryId))
        {
            throw new MasterCategoryNotFoundException(chosenCategoryId);
        }

        var uploadId = Guid.CreateVersion7();
        var upload = FileUpload.Create(
            uploadId,
            masterId,
            purpose,
            categoryId,
            fileName,
            contentType,
            sizeInBytes,
            string.Create(CultureInfo.InvariantCulture, $"{masterId:N}/{uploadId:N}"),
            timeProvider.GetUtcNow());

        await fileStorage.UploadFileAsync(content, _storage.StagingBucket, upload.StagingKey, upload.ContentType, cancellationToken);
        await uploads.CreateAsync(upload, cancellationToken);

        logger.LogInformation("Upload {UploadId} of master {MasterId} staged for the check", uploadId, masterId);
        return new StagedUpload(upload.ToResponse(), masterId);
    }

    private async Task<MemoryStream?> TryDownloadStagedAsync(
        FileUpload upload,
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
        FileUpload upload,
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

    private (string Bucket, string Key) PermanentLocationOf(FileUpload upload)
    {
        var extension = FileSignatures.ExtensionFor(upload.ContentType);

        return upload.Purpose is UploadPurpose.Avatar
            ? (_storage.AvatarsBucket, string.Create(CultureInfo.InvariantCulture, $"{upload.MasterId:N}/{upload.Id:N}{extension}"))
            : (_storage.DocumentsBucket, string.Create(CultureInfo.InvariantCulture, $"{upload.MasterId:N}/{upload.Id:N}/{StoredFileName}{extension}"));
    }

    private Guid? Attach(
        Master master,
        FileUpload upload,
        string key,
        DateTimeOffset now,
        List<StoredFileResponse> obsoleteFiles)
    {
        switch (upload.Purpose)
        {
            case UploadPurpose.Avatar:
                if (master.Avatar is not null)
                {
                    obsoleteFiles.Add(new StoredFileResponse(_storage.AvatarsBucket, master.Avatar.StorageKey));
                }

                master.ChangeAvatar(Avatar.Of(key, upload.ContentType, upload.SizeInBytes), now);
                return null;

            case UploadPurpose.IdentityDocument:
                return master.UploadIdentityDocument(key, upload.ContentType, upload.SizeInBytes, now).Id;

            case UploadPurpose.QualificationDocument:
                return master.UploadQualificationDocument(upload.CategoryId!.Value, key, upload.ContentType, upload.SizeInBytes, now).Id;

            default:
                throw new InvalidFileUploadException($"Upload purpose '{upload.Purpose}' is unknown.");
        }
    }

    private async Task<UploadProcessingResult> FailAsync(
        FileUpload upload,
        string reason,
        DateTimeOffset now,
        StoredFileResponse stagingFile,
        CancellationToken cancellationToken)
    {
        upload.Fail(Reason.Of(reason), now);
        await uploads.UpdateAsync(upload, cancellationToken);

        logger.LogInformation("Upload {UploadId} of master {MasterId} rejected", upload.Id, upload.MasterId);
        return new UploadProcessingResult(upload.ToResponse(), upload.MasterId, null, [stagingFile]);
    }

    private async Task<MemoryStream> DownloadAsync(
        string bucket,
        string key,
        CancellationToken cancellationToken)
    {
        try
        {
            return await fileStorage.DownloadFileAsync(bucket, key, cancellationToken);
        }
        catch (FileNotFoundInStorageException)
        {
            throw new NotFoundException("The file was not found in the storage.");
        }
    }
}