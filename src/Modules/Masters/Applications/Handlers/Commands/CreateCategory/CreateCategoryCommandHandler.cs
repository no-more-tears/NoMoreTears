namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler(
    ICategoryService categoryService,
    IEventPublisher<IMastersOutbox> eventPublisher) : ICommandHandler<CreateCategoryCommand, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await categoryService.CreateAsync(request.Name, request.Description, cancellationToken);

        await eventPublisher.PublishAsync(
            new CategoryCreatedEvent(category.Id, category.Name, category.Description, category.CreatedAt!.Value),
            cancellationToken);

        return category;
    }
}
