namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RemoveAvatar;

public sealed class RemoveAvatarCommandHandler(
    IMasterService masterService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<RemoveAvatarCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        RemoveAvatarCommand request,
        CancellationToken cancellationToken)
    {
        var removal = await masterService.RemoveAvatarAsync(request.MasterId, cancellationToken);
        var changedAt = MastersEvents.ChangedAt(removal.Master);

        await eventPublisher.PublishAsync(new MasterAvatarChangedEvent(removal.Master.Id, false, changedAt), cancellationToken);
        await eventPublisher.PublishAsync(new MasterFilesObsoletedEvent(removal.Master.Id, [removal.RemovedFile], changedAt), cancellationToken);

        return removal.Master;
    }
}
