namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ActivateCategory;

public sealed class ActivateCategoryCommandHandler(
    ICategoryService categoryService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<ActivateCategoryCommand, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(
        ActivateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await categoryService.ActivateAsync(request.CategoryId, cancellationToken);

        await eventPublisher.PublishAsync(new CategoryActivatedEvent(category.Id, category.LastUpdatedAt!.Value), cancellationToken);

        return category;
    }
}
