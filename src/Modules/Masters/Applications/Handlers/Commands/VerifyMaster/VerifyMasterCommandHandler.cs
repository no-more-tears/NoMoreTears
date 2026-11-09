namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyMaster;

public sealed class VerifyMasterCommandHandler(
    IVerificationService verificationService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<VerifyMasterCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        VerifyMasterCommand request,
        CancellationToken cancellationToken)
    {
        var master = await verificationService.VerifyAsync(request.ReviewerId, request.MasterId, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterVerifiedEvent(master.Id, request.ReviewerId, MastersEvents.VerifiedCategoryIds(master), MastersEvents.ChangedAt(master)),
            cancellationToken);
        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);

        return master;
    }
}
