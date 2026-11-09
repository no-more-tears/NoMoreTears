namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RemoveMasterCategory;

public sealed class RemoveMasterCategoryCommandHandler(
    IMasterService masterService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<RemoveMasterCategoryCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        RemoveMasterCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var master = await masterService.RemoveCategoryAsync(request.MasterId, request.CategoryId, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterCategoryRemovedEvent(master.Id, request.CategoryId, MastersEvents.ChangedAt(master)),
            cancellationToken);
        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);

        return master;
    }
}
