namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMaster;

public sealed class GetMasterQueryHandler(
    IMasterService masterService) : IQueryHandler<GetMasterQuery, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        GetMasterQuery request,
        CancellationToken cancellationToken)
    {
        return await masterService.GetAsync(request.MasterId, cancellationToken);
    }
}
