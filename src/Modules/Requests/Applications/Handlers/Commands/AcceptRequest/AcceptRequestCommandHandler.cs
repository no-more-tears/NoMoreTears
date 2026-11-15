namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.AcceptRequest;

public sealed class AcceptRequestCommandHandler(
    IExecutionService executionService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<AcceptRequestCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        AcceptRequestCommand request,
        CancellationToken cancellationToken)
    {
        var response = await executionService.AcceptAsync(
            request.MasterId,
            request.RequestId,
            request.ArrivalIntervalInHours,
            cancellationToken);
        var assignment = response.Assignment!;

        await eventPublisher.PublishTransitionAsync(
            new RequestAcceptedEvent(
                response.Id,
                response.ClientId,
                assignment.MasterId,
                response.CategoryId,
                assignment.AcceptedAt,
                assignment.ExpectedArrivalAt,
                RequestsEvents.ChangedAt(response)),
            response,
            cancellationToken);

        return response;
    }
}