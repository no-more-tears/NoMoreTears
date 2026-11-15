namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequest;

public sealed class GetOwnRequestQueryHandler(
    IRequestService requestService) : IQueryHandler<GetOwnRequestQuery, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        GetOwnRequestQuery request,
        CancellationToken cancellationToken)
    {
        return await requestService.GetAsync(request.ClientId, request.RequestId, cancellationToken);
    }
}