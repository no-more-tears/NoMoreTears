namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectMaster;

public sealed class RejectMasterCommandHandler(
    IVerificationService verificationService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<RejectMasterCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        RejectMasterCommand request,
        CancellationToken cancellationToken)
    {
        var master = await verificationService.RejectAsync(request.ReviewerId, request.MasterId, request.Reason, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterVerificationRejectedEvent(master.Id, request.ReviewerId, MastersEvents.ReviewReason(master.VerificationReview), MastersEvents.ChangedAt(master)),
            cancellationToken);

        return master;
    }
}
