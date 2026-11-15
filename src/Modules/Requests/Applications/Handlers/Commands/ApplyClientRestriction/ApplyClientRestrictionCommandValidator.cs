using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ApplyClientRestriction;

public sealed class ApplyClientRestrictionCommandValidator : AbstractValidator<ApplyClientRestrictionCommand>
{
    public ApplyClientRestrictionCommandValidator()
    {
        RuleFor(command => command.RestrictionId).RequiredId("Restriction id");

        RuleFor(command => command.ClientId).RequiredId("Client id");

        RuleFor(command => command.Kind).NotEmpty().WithMessage("Restriction kind is required.");
    }
}