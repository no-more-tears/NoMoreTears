namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetCurrentMaster;

public sealed class GetCurrentMasterQueryHandler(
    IMasterService masterService) : IQueryHandler<GetCurrentMasterQuery, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        GetCurrentMasterQuery request,
        CancellationToken cancellationToken)
    {
        return await masterService.GetAsync(request.MasterId, cancellationToken);
    }
}
