namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequestMediaFile;

/// <summary>
///     Returns the content of a photo of any request for arbiters.
/// </summary>
public sealed record GetRequestMediaFileQuery(
    Guid RequestId,
    Guid MediaFileId) : IQuery<FileContentResponse>;