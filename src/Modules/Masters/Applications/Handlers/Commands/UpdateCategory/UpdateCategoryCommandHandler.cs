namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandHandler(
    ICategoryService categoryService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<UpdateCategoryCommand, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await categoryService.UpdateAsync(request.CategoryId, request.Name, request.Description, cancellationToken);

        await eventPublisher.PublishAsync(
            new CategoryUpdatedEvent(category.Id, category.Name, category.Description, category.LastUpdatedAt!.Value),
            cancellationToken);

        return category;
    }
}
