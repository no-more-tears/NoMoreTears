using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequestHistory;

/// <summary>
///     Returns the status history of a request of the client (FR-REQ-08).
/// </summary>
public sealed record GetOwnRequestHistoryQuery(
    Guid ClientId,
    Guid RequestId) : IQuery<IReadOnlyList<StatusChangeResponse>>, IRequestClientRequest;