namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterRequestHistory;

public sealed class GetMasterRequestHistoryQueryHandler(
    IExecutionService executionService) : IQueryHandler<GetMasterRequestHistoryQuery, IReadOnlyList<StatusChangeResponse>>
{
    public async Task<IReadOnlyList<StatusChangeResponse>> Handle(
        GetMasterRequestHistoryQuery request,
        CancellationToken cancellationToken)
    {
        return await executionService.GetHistoryAsync(request.MasterId, request.RequestId, cancellationToken);
    }
}