namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectServiceArea;

public sealed class RejectServiceAreaCommandHandler(
    IVerificationService verificationService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<RejectServiceAreaCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        RejectServiceAreaCommand request,
        CancellationToken cancellationToken)
    {
        var master = await verificationService.RejectServiceAreaAsync(request.ReviewerId, request.MasterId, request.Reason, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterServiceAreaRejectedEvent(master.Id, request.ReviewerId, MastersEvents.ReviewReason(master.ServiceAreaReview), MastersEvents.ChangedAt(master)),
            cancellationToken);
        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);

        return master;
    }
}
