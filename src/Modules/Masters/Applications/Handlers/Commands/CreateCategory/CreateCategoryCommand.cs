namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description) : ICommand<CategoryResponse>;
