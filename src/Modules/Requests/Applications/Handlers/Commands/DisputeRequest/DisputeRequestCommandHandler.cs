namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DisputeRequest;

public sealed class DisputeRequestCommandHandler(
    IArbitrationService arbitrationService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<DisputeRequestCommand>
{
    public async Task Handle(
        DisputeRequestCommand request,
        CancellationToken cancellationToken)
    {
        var response = await arbitrationService.DisputeAsync(request.RequestId, request.IncidentId, cancellationToken);

        if (response is null)
        {
            return;
        }

        await eventPublisher.PublishTransitionAsync(
            new RequestDisputedEvent(
                response.Id,
                response.ClientId,
                RequestsEvents.MasterOf(response),
                request.IncidentId,
                response.StatusBeforeDispute!,
                RequestsEvents.ChangedAt(response)),
            response,
            cancellationToken);
    }
}