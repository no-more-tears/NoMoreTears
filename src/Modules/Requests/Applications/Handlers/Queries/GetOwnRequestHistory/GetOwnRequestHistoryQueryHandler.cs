namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequestHistory;

public sealed class GetOwnRequestHistoryQueryHandler(
    IRequestService requestService) : IQueryHandler<GetOwnRequestHistoryQuery, IReadOnlyList<StatusChangeResponse>>
{
    public async Task<IReadOnlyList<StatusChangeResponse>> Handle(
        GetOwnRequestHistoryQuery request,
        CancellationToken cancellationToken)
    {
        return await requestService.GetHistoryAsync(request.ClientId, request.RequestId, cancellationToken);
    }
}