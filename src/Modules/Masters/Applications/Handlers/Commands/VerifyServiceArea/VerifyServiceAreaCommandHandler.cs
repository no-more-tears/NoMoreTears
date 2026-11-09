namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyServiceArea;

public sealed class VerifyServiceAreaCommandHandler(
    IVerificationService verificationService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<VerifyServiceAreaCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        VerifyServiceAreaCommand request,
        CancellationToken cancellationToken)
    {
        var master = await verificationService.VerifyServiceAreaAsync(request.ReviewerId, request.MasterId, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterServiceAreaVerifiedEvent(master.Id, request.ReviewerId, MastersEvents.ChangedAt(master)),
            cancellationToken);
        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);

        return master;
    }
}
