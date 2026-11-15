using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnMediaFile;

/// <summary>
///     Returns the content of a photo of a request of the client.
/// </summary>
public sealed record GetOwnMediaFileQuery(
    Guid ClientId,
    Guid RequestId,
    Guid MediaFileId) : IQuery<FileContentResponse>, IRequestClientRequest;