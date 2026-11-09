namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ArchiveCategory;

public sealed class ArchiveCategoryCommandHandler(
    ICategoryService categoryService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<ArchiveCategoryCommand, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(
        ArchiveCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await categoryService.ArchiveAsync(request.CategoryId, cancellationToken);

        await eventPublisher.PublishAsync(new CategoryArchivedEvent(category.Id, category.LastUpdatedAt!.Value), cancellationToken);

        return category;
    }
}
