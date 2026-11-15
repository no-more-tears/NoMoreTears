using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterRequests;

public sealed class GetMasterRequestsQueryHandler(
    IExecutionService executionService) : IQueryHandler<GetMasterRequestsQuery, PagedResponse<RequestSummaryResponse>>
{
    public async Task<PagedResponse<RequestSummaryResponse>> Handle(
        GetMasterRequestsQuery request,
        CancellationToken cancellationToken)
    {
        return await executionService.GetPageAsync(request.MasterId, request.Status, request.PageNumber, request.PageSize, cancellationToken);
    }
}