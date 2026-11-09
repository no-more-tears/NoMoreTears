namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ApplyTemporaryMeasure;

public sealed class ApplyTemporaryMeasureCommandHandler(
    ISanctionService sanctionService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<ApplyTemporaryMeasureCommand>
{
    public async Task Handle(
        ApplyTemporaryMeasureCommand request,
        CancellationToken cancellationToken)
    {
        var outcome = await sanctionService.ApplyMeasureAsync(
            request.MasterId,
            request.MeasureId,
            request.Rule,
            request.SuspendsFeed,
            request.HoldsVerification,
            request.AppliedAt,
            request.ExpiresAt,
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
                new MasterSanctionAppliedEvent(master.Id, suspension.Id, suspension.Kind, suspension.ImposedAt, suspension.ExpiresAt, changedAt),
                cancellationToken);
        }

        if (outcome.HoldChanged)
        {
            await eventPublisher.PublishAsync(
                new MasterVerificationHoldPlacedEvent(master.Id, master.VerificationHold!.Reason, changedAt),
                cancellationToken);
        }

        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);
    }
}
