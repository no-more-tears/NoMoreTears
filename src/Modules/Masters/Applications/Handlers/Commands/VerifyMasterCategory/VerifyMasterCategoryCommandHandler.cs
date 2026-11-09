namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyMasterCategory;

public sealed class VerifyMasterCategoryCommandHandler(
    IVerificationService verificationService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<VerifyMasterCategoryCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        VerifyMasterCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var master = await verificationService.VerifyCategoryAsync(request.ReviewerId, request.MasterId, request.CategoryId, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterCategoryVerifiedEvent(master.Id, request.CategoryId, request.ReviewerId, MastersEvents.ChangedAt(master)),
            cancellationToken);
        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);

        return master;
    }
}
