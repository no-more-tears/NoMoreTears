namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RefuseRequest;

public sealed class RefuseRequestCommandHandler(
    IExecutionService executionService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<RefuseRequestCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        RefuseRequestCommand request,
        CancellationToken cancellationToken)
    {
        var response = await executionService.RefuseAsync(request.MasterId, request.RequestId, request.Reason, cancellationToken);

        await eventPublisher.PublishTransitionAsync(RequestsEvents.Failed(response), response, cancellationToken);

        return response;
    }
}