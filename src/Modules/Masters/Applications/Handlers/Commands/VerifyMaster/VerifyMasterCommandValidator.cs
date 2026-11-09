using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyMaster;

public sealed class VerifyMasterCommandValidator : AbstractValidator<VerifyMasterCommand>
{
    public VerifyMasterCommandValidator()
    {
        RuleFor(command => command.ReviewerId).RequiredId("Reviewer id");

        RuleFor(command => command.MasterId).RequiredId("Master id");
    }
}
