using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.UploadMediaFile;

/// <summary>
///     Receives a photo for a draft; it is checked and attached asynchronously.
/// </summary>
public sealed record UploadMediaFileCommand(
    Guid ClientId,
    Guid RequestId,
    Stream Content,
    string FileName,
    string ContentType,
    long SizeInBytes) : ICommand<MediaUploadResponse>, IRequestClientRequest;