namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ActivateCategory;

public sealed record ActivateCategoryCommand(
    Guid CategoryId) : ICommand<CategoryResponse>;
