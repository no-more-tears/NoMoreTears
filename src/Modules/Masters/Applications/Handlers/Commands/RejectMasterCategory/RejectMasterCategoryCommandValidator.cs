using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectMasterCategory;

public sealed class RejectMasterCategoryCommandValidator : AbstractValidator<RejectMasterCategoryCommand>
{
    public RejectMasterCategoryCommandValidator()
    {
        RuleFor(command => command.ReviewerId).RequiredId("Reviewer id");

        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.CategoryId).RequiredId("Category id");

        RuleFor(command => command.Reason).RequiredReason();
    }
}
