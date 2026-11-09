namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetCategories;

public sealed class GetCategoriesQueryHandler(
    ICategoryService categoryService) : IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryResponse>>
{
    public async Task<IReadOnlyList<CategoryResponse>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        return await categoryService.GetAllAsync(request.IncludeArchived, cancellationToken);
    }
}
