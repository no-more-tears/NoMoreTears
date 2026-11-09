namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RemoveDocument;

public sealed class RemoveDocumentCommandHandler(
    IMasterService masterService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<RemoveDocumentCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        RemoveDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var removal = await masterService.RemoveDocumentAsync(request.MasterId, request.DocumentId, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterFilesObsoletedEvent(removal.Master.Id, [removal.RemovedFile], MastersEvents.ChangedAt(removal.Master)),
            cancellationToken);

        return removal.Master;
    }
}
