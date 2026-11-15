using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequests;

/// <summary>
///     Returns a page of requests of the client, newest first, optionally filtered by status.
/// </summary>
public sealed record GetOwnRequestsQuery(
    Guid ClientId,
    string? Status,
    int PageNumber,
    int PageSize) : IQuery<PagedResponse<RequestSummaryResponse>>, IPagedQuery, IRequestClientRequest;