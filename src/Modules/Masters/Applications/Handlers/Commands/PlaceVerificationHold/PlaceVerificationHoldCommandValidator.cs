using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.PlaceVerificationHold;

public sealed class PlaceVerificationHoldCommandValidator : AbstractValidator<PlaceVerificationHoldCommand>
{
    public PlaceVerificationHoldCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.Reason).RequiredReason();
    }
}
