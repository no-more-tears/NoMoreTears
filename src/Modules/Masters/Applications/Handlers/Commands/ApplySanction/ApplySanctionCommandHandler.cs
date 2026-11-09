namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ApplySanction;

public sealed class ApplySanctionCommandHandler(
    ISanctionService sanctionService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<ApplySanctionCommand>
{
    public async Task Handle(
        ApplySanctionCommand request,
        CancellationToken cancellationToken)
    {
        var outcome = await sanctionService.ApplySanctionAsync(
            request.MasterId,
            request.SanctionId,
            request.Kind,
            request.ImposedAt,
            request.ExpiresAt,
            cancellationToken);

        if (outcome is null)
        {
            return;
        }

        var (master, sanction) = (outcome.Master, outcome.Sanction);
        var changedAt = MastersEvents.ChangedAt(master);

        await eventPublisher.PublishAsync(
            new MasterSanctionAppliedEvent(master.Id, sanction.Id, sanction.Kind, sanction.ImposedAt, sanction.ExpiresAt, changedAt),
            cancellationToken);

        if (outcome.VerificationChanged)
        {
            await eventPublisher.PublishAsync(
                new MasterVerificationRevokedEvent(master.Id, sanction.Id, master.RevokedAt!.Value, master.ReverificationAllowedFrom!.Value, changedAt),
                cancellationToken);
        }

        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);
    }
}
