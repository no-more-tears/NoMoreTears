namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.MarkRequestAsDone;

public sealed class MarkRequestAsDoneCommandHandler(
    IExecutionService executionService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<MarkRequestAsDoneCommand, RequestResponse>
{
    public async Task<RequestResponse> Handle(
        MarkRequestAsDoneCommand request,
        CancellationToken cancellationToken)
    {
        var response = await executionService.MarkAsDoneAsync(request.MasterId, request.RequestId, cancellationToken);

        await eventPublisher.PublishTransitionAsync(
            new RequestMarkedAsDoneEvent(
                response.Id,
                response.ClientId,
                response.Assignment!.MasterId,
                response.ConfirmationDeadline!.Value,
                RequestsEvents.ChangedAt(response)),
            response,
            cancellationToken);

        return response;
    }
}