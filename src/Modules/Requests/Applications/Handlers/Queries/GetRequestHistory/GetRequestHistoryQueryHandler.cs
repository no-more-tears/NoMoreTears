namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequestHistory;

public sealed class GetRequestHistoryQueryHandler(
    IArbitrationService arbitrationService) : IQueryHandler<GetRequestHistoryQuery, IReadOnlyList<StatusChangeResponse>>
{
    public async Task<IReadOnlyList<StatusChangeResponse>> Handle(
        GetRequestHistoryQuery request,
        CancellationToken cancellationToken)
    {
        return await arbitrationService.GetHistoryAsync(request.RequestId, cancellationToken);
    }
}