using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RejectMaster;

public sealed class RejectMasterCommandValidator : AbstractValidator<RejectMasterCommand>
{
    public RejectMasterCommandValidator()
    {
        RuleFor(command => command.ReviewerId).RequiredId("Reviewer id");

        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.Reason).RequiredReason();
    }
}
