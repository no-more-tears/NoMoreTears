using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterRequestHistory;

/// <summary>
///     Returns the status history of a request assigned to the master (FR-REQ-08).
/// </summary>
public sealed record GetMasterRequestHistoryQuery(
    Guid MasterId,
    Guid RequestId) : IQuery<IReadOnlyList<StatusChangeResponse>>, IRequestMasterRequest;