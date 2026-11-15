using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RemoveMediaFile;

/// <summary>
///     Detaches a photo from a draft; the file is removed from the storage after the commit.
/// </summary>
public sealed record RemoveMediaFileCommand(
    Guid ClientId,
    Guid RequestId,
    Guid MediaFileId) : ICommand<RequestResponse>, IRequestClientRequest;