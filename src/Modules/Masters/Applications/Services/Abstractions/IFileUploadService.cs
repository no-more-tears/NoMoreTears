using NoMoreTears.Modules.Masters.Applications.Services.Results;
using NoMoreTears.Modules.Masters.Contracts.Responses;

namespace NoMoreTears.Modules.Masters.Applications.Services.Abstractions;

/// <summary>
///     Asynchronous acceptance of files.
/// </summary>
public interface IFileUploadService
{
    Task<StagedUpload> StageAvatarAsync(
        Guid masterId,
        Stream content,
        string fileName,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken);

    Task<StagedUpload> StageDocumentAsync(
        Guid masterId,
        string kind,
        Guid? categoryId,
        Stream content,
        string fileName,
        string contentType,
        long sizeInBytes,
        CancellationToken cancellationToken);

    Task<FileUploadResponse> GetAsync(
        Guid masterId,
        Guid uploadId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Checks a staged file and attaches it; returns <c>null</c> when the upload was already processed.
    /// </summary>
    Task<UploadProcessingResult?> ProcessAsync(
        Guid uploadId,
        CancellationToken cancellationToken);

    Task<FileContentResponse> OpenAvatarAsync(
        Guid masterId,
        CancellationToken cancellationToken);

    Task<FileContentResponse> OpenDocumentAsync(
        Guid masterId,
        Guid documentId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Removes files that are no longer referenced; missing files are ignored.
    /// </summary>
    Task DeleteFilesAsync(
        IReadOnlyCollection<StoredFileResponse> files,
        CancellationToken cancellationToken);
}