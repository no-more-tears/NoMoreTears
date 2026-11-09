namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.LiftTemporaryMeasure;

public sealed class LiftTemporaryMeasureCommandHandler(
    ISanctionService sanctionService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<LiftTemporaryMeasureCommand>
{
    public async Task Handle(
        LiftTemporaryMeasureCommand request,
        CancellationToken cancellationToken)
    {
        var outcome = await sanctionService.LiftMeasureAsync(
            request.MasterId,
            request.MeasureId,
            request.SuspendsFeed,
            request.HoldsVerification,
            request.LiftedAt,
            cancellationToken);

        if (outcome is null)
        {
            return;
        }

        var master = outcome.Master;
        var changedAt = MastersEvents.ChangedAt(master);

        if (outcome.Suspension is { } suspension)
        {
            await eventPublisher.PublishAsync(
                new MasterSanctionLiftedEvent(master.Id, suspension.Id, suspension.Kind, suspension.LiftedAt!.Value, changedAt),
                cancellationToken);
        }

        if (outcome.HoldChanged)
        {
            await eventPublisher.PublishAsync(new MasterVerificationHoldReleasedEvent(master.Id, changedAt), cancellationToken);
        }

        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);
    }
}
