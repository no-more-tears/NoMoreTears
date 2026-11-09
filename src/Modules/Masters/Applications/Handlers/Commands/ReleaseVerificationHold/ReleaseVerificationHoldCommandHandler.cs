namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ReleaseVerificationHold;

public sealed class ReleaseVerificationHoldCommandHandler(
    IVerificationService verificationService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<ReleaseVerificationHoldCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        ReleaseVerificationHoldCommand request,
        CancellationToken cancellationToken)
    {
        var master = await verificationService.ReleaseHoldAsync(request.MasterId, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterVerificationHoldReleasedEvent(master.Id, MastersEvents.ChangedAt(master)),
            cancellationToken);

        return master;
    }
}
