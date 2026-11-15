namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DiscardRequest;

public sealed class DiscardRequestCommandHandler(
    IRequestService requestService,
    IEventPublisher<IRequestsOutbox> eventPublisher) : ICommandHandler<DiscardRequestCommand>
{
    public async Task Handle(
        DiscardRequestCommand request,
        CancellationToken cancellationToken)
    {
        var removal = await requestService.DiscardAsync(request.ClientId, request.RequestId, cancellationToken);
        var changedAt = RequestsEvents.ChangedAt(removal.Request);

        await eventPublisher.PublishAsync(new RequestDiscardedEvent(removal.Request.Id, removal.Request.ClientId, changedAt), cancellationToken);
        await eventPublisher.PublishObsoleteFilesAsync(removal.Request.Id, removal.RemovedFiles, changedAt, cancellationToken);
    }
}