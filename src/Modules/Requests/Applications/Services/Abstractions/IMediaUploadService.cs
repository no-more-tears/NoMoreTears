using NoMoreTears.Modules.Requests.Applications.Services.Results;
using NoMoreTears.Modules.Requests.Contracts.Responses;

namespace NoMoreTears.Modules.Requests.Applications.Services.Abstractions;

/// <summary>
///     Asynchronous acceptance of photos and access to stored photos.
/// </summary>
public interface IMediaUploadService
{
    Task<StagedMediaUpload> StageAsync(
        Guid clientId,
        Guid requestId,
        Stream content,
        string fileName,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken);

    Task<MediaUploadResponse> GetAsync(
        Guid clientId,
        Guid requestId,
        Guid uploadId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Checks a staged photo and attaches it.
    /// </summary>
    Task<MediaUploadProcessingResult?> ProcessAsync(
        Guid uploadId,
        CancellationToken cancellationToken);

    Task<FileContentResponse> OpenForClientAsync(
        Guid clientId,
        Guid requestId,
        Guid mediaFileId,
        CancellationToken cancellationToken);

    Task<FileContentResponse> OpenForMasterAsync(
        Guid masterId,
        Guid requestId,
        Guid mediaFileId,
        CancellationToken cancellationToken);

    Task<FileContentResponse> OpenForSupportAsync(
        Guid requestId,
        Guid mediaFileId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Removes files that are no longer referenced; missing files are ignored.
    /// </summary>
    Task DeleteFilesAsync(
        IReadOnlyCollection<StoredFileResponse> files,
        CancellationToken cancellationToken);
}