using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.LiftClientRestriction;

public sealed class LiftClientRestrictionCommandValidator : AbstractValidator<LiftClientRestrictionCommand>
{
    public LiftClientRestrictionCommandValidator()
    {
        RuleFor(command => command.RestrictionId).RequiredId("Restriction id");
    }
}