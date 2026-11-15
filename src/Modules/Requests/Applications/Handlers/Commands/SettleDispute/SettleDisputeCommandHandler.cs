namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.SettleDispute;

public sealed class SettleDisputeCommandHandler(
    IArbitrationService arbitrationService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<SettleDisputeCommand>
{
    public async Task Handle(
        SettleDisputeCommand request,
        CancellationToken cancellationToken)
    {
        var response = await arbitrationService.SettleDisputeAsync(request.RequestId, request.IncidentId, cancellationToken);

        if (response is null)
        {
            return;
        }

        await eventPublisher.PublishTransitionAsync(
            new RequestDisputeSettledEvent(
                response.Id,
                response.ClientId,
                RequestsEvents.MasterOf(response),
                request.IncidentId,
                response.Status,
                RequestsEvents.ChangedAt(response)),
            response,
            cancellationToken);
    }
}