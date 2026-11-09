namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetCategory;

public sealed record GetCategoryQuery(
    Guid CategoryId) : IQuery<CategoryResponse>;
