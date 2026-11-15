namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterRequest;

public sealed class GetMasterRequestQueryHandler(
    IExecutionService executionService) : IQueryHandler<GetMasterRequestQuery, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        GetMasterRequestQuery request,
        CancellationToken cancellationToken)
    {
        return await executionService.GetAsync(request.MasterId, request.RequestId, cancellationToken);
    }
}