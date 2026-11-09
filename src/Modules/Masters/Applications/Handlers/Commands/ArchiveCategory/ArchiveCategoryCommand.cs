namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ArchiveCategory;

public sealed record ArchiveCategoryCommand(
    Guid CategoryId) : ICommand<CategoryResponse>;
