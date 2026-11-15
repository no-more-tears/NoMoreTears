using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequests;

/// <summary>
///     Returns a page of all requests for arbiters, latest change first, with optional filters.
/// </summary>
public sealed record GetRequestsQuery(
    string? Status,
    Guid? ClientId,
    Guid? MasterId,
    int PageNumber,
    int PageSize) : IQuery<PagedResponse<RequestSummaryResponse>>, IPagedQuery;