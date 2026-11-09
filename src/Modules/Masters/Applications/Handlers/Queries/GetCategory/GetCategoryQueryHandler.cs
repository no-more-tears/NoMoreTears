namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetCategory;

public sealed class GetCategoryQueryHandler(
    ICategoryService categoryService) : IQueryHandler<GetCategoryQuery, CategoryResponse>
{
    public async Task<CategoryResponse> Handle(
        GetCategoryQuery request,
        CancellationToken cancellationToken)
    {
        return await categoryService.GetAsync(request.CategoryId, cancellationToken);
    }
}
