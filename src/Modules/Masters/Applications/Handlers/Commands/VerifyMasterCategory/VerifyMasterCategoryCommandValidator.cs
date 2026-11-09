using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyMasterCategory;

public sealed class VerifyMasterCategoryCommandValidator : AbstractValidator<VerifyMasterCategoryCommand>
{
    public VerifyMasterCategoryCommandValidator()
    {
        RuleFor(command => command.ReviewerId).RequiredId("Reviewer id");

        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.CategoryId).RequiredId("Category id");
    }
}
