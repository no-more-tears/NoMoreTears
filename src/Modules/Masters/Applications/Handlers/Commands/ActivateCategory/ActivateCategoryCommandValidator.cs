using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ActivateCategory;

public sealed class ActivateCategoryCommandValidator : AbstractValidator<ActivateCategoryCommand>
{
    public ActivateCategoryCommandValidator()
    {
        RuleFor(command => command.CategoryId).RequiredId("Category id");
    }
}
