using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterMediaFile;

/// <summary>
///     Returns the content of a photo of a request the master may see.
/// </summary>
public sealed record GetMasterMediaFileQuery(
    Guid MasterId,
    Guid RequestId,
    Guid MediaFileId) : IQuery<FileContentResponse>, IRequestMasterRequest;