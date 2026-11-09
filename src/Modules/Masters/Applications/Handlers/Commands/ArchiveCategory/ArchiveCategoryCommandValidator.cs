using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ArchiveCategory;

public sealed class ArchiveCategoryCommandValidator : AbstractValidator<ArchiveCategoryCommand>
{
    public ArchiveCategoryCommandValidator()
    {
        RuleFor(command => command.CategoryId).RequiredId("Category id");
    }
}
