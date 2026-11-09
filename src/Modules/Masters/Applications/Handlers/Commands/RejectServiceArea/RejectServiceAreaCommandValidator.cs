using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectServiceArea;

public sealed class RejectServiceAreaCommandValidator : AbstractValidator<RejectServiceAreaCommand>
{
    public RejectServiceAreaCommandValidator()
    {
        RuleFor(command => command.ReviewerId).RequiredId("Reviewer id");

        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.Reason).RequiredReason();
    }
}
