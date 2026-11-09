using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.AddMasterCategory;

public sealed class AddMasterCategoryCommandValidator : AbstractValidator<AddMasterCategoryCommand>
{
    public AddMasterCategoryCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.CategoryId).RequiredId("Category id");
    }
}
