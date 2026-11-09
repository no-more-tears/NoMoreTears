namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectMasterCategory;

public sealed class RejectMasterCategoryCommandHandler(
    IVerificationService verificationService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<RejectMasterCategoryCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        RejectMasterCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var master = await verificationService.RejectCategoryAsync(request.ReviewerId, request.MasterId, request.CategoryId, request.Reason, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterCategoryRejectedEvent(
                master.Id,
                request.CategoryId,
                request.ReviewerId,
                MastersEvents.ReviewReason(MastersEvents.CategoryReview(master, request.CategoryId)),
                MastersEvents.ChangedAt(master)),
            cancellationToken);
        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);

        return master;
    }
}
