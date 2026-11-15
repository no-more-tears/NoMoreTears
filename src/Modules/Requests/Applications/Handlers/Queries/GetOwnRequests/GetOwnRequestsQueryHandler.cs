using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequests;

public sealed class GetOwnRequestsQueryHandler(
    IRequestService requestService) : IQueryHandler<GetOwnRequestsQuery, PagedResponse<RequestSummaryResponse>>
{
    public async Task<PagedResponse<RequestSummaryResponse>> Handle(
        GetOwnRequestsQuery request,
        CancellationToken cancellationToken)
    {
        return await requestService.GetPageAsync(request.ClientId, request.Status, request.PageNumber, request.PageSize, cancellationToken);
    }
}