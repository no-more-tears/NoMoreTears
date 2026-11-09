namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetCategories;

/// <summary>
///     Returns the catalog; archived categories are included only on request of a PlatformAdministrator.
/// </summary>
public sealed record GetCategoriesQuery(
    bool IncludeArchived) : IQuery<IReadOnlyList<CategoryResponse>>;
