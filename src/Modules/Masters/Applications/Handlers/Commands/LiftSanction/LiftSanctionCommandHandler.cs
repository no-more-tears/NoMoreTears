namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.LiftSanction;

public sealed class LiftSanctionCommandHandler(
    ISanctionService sanctionService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<LiftSanctionCommand>
{
    public async Task Handle(
        LiftSanctionCommand request,
        CancellationToken cancellationToken)
    {
        var outcome = await sanctionService.LiftSanctionAsync(request.MasterId, request.SanctionId, request.LiftedAt, cancellationToken);

        if (outcome is null)
        {
            return;
        }

        var (master, sanction) = (outcome.Master, outcome.Sanction);
        var changedAt = MastersEvents.ChangedAt(master);

        await eventPublisher.PublishAsync(
            new MasterSanctionLiftedEvent(master.Id, sanction.Id, sanction.Kind, sanction.LiftedAt!.Value, changedAt),
            cancellationToken);

        if (outcome.VerificationChanged)
        {
            await eventPublisher.PublishAsync(new MasterVerificationRestoredEvent(master.Id, sanction.Id, changedAt), cancellationToken);
        }

        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);
    }
}
