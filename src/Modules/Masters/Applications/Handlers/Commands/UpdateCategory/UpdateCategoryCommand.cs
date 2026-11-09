namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string? Description) : ICommand<CategoryResponse>;
