using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterRequests;

/// <summary>
///     Returns a page of requests accepted by the master, latest acceptance first.
/// </summary>
public sealed record GetMasterRequestsQuery(
    Guid MasterId,
    string? Status,
    int PageNumber,
    int PageSize) : IQuery<PagedResponse<RequestSummaryResponse>>, IPagedQuery, IRequestMasterRequest;