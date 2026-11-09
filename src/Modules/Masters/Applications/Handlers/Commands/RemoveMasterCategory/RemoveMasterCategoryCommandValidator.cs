using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RemoveMasterCategory;

public sealed class RemoveMasterCategoryCommandValidator : AbstractValidator<RemoveMasterCategoryCommand>
{
    public RemoveMasterCategoryCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.CategoryId).RequiredId("Category id");
    }
}
