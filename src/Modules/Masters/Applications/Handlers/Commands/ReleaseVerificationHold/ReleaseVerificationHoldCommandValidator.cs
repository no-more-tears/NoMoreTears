using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ReleaseVerificationHold;

public sealed class ReleaseVerificationHoldCommandValidator : AbstractValidator<ReleaseVerificationHoldCommand>
{
    public ReleaseVerificationHoldCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");
    }
}
