namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeAvailability;

public sealed class ChangeAvailabilityCommandHandler(
    IMasterService masterService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<ChangeAvailabilityCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        ChangeAvailabilityCommand request,
        CancellationToken cancellationToken)
    {
        var master = await masterService.ChangeAvailabilityAsync(request.MasterId, request.IsAvailable, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterAvailabilityChangedEvent(master.Id, master.IsAvailable, MastersEvents.ChangedAt(master)),
            cancellationToken);
        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);

        return master;
    }
}
