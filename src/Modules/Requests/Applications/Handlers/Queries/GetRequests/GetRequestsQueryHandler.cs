using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequests;

public sealed class GetRequestsQueryHandler(
    IArbitrationService arbitrationService) : IQueryHandler<GetRequestsQuery, PagedResponse<RequestSummaryResponse>>
{
    public async Task<PagedResponse<RequestSummaryResponse>> Handle(
        GetRequestsQuery request,
        CancellationToken cancellationToken)
    {
        return await arbitrationService.GetPageAsync(
            request.Status,
            request.ClientId,
            request.MasterId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}