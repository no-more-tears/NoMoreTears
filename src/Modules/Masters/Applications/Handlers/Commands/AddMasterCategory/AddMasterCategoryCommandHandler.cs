namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.AddMasterCategory;

public sealed class AddMasterCategoryCommandHandler(
    IMasterService masterService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<AddMasterCategoryCommand, MasterResponse>
{
    public async Task<MasterResponse> Handle(
        AddMasterCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var master = await masterService.AddCategoryAsync(request.MasterId, request.CategoryId, cancellationToken);

        await eventPublisher.PublishAsync(
            new MasterCategoryAddedEvent(master.Id, request.CategoryId, MastersEvents.ChangedAt(master)),
            cancellationToken);
        await eventPublisher.PublishAsync(MastersEvents.AccessUpdated(master), cancellationToken);

        return master;
    }
}
