using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMediaUpload;

/// <summary>
///     Returns the status of a photo uploaded by the client.
/// </summary>
public sealed record GetMediaUploadQuery(
    Guid ClientId,
    Guid RequestId,
    Guid UploadId) : IQuery<MediaUploadResponse>, IRequestClientRequest;