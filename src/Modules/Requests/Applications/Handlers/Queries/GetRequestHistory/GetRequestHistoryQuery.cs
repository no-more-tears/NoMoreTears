namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequestHistory;

/// <summary>
///     Returns the status history of any request for arbiters (FR-REQ-08).
/// </summary>
public sealed record GetRequestHistoryQuery(
    Guid RequestId) : IQuery<IReadOnlyList<StatusChangeResponse>>;