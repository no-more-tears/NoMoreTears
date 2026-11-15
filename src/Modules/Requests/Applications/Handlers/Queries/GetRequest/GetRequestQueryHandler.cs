namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequest;

public sealed class GetRequestQueryHandler(
    IArbitrationService arbitrationService) : IQueryHandler<GetRequestQuery, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        GetRequestQuery request,
        CancellationToken cancellationToken)
    {
        return await arbitrationService.GetAsync(request.RequestId, cancellationToken);
    }
}