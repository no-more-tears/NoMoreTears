using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMasters;

public sealed class GetMastersQueryHandler(
    IMasterService masterService) : IQueryHandler<GetMastersQuery, PagedResponse<MasterSummaryResponse>>
{
    public async Task<PagedResponse<MasterSummaryResponse>> Handle(
        GetMastersQuery request,
        CancellationToken cancellationToken)
    {
        return await masterService.GetPageAsync(request.Verification, request.PageNumber, request.PageSize, cancellationToken);
    }
}
