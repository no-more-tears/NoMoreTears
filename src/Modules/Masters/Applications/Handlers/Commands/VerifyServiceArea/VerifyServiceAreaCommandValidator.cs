using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.VerifyServiceArea;

public sealed class VerifyServiceAreaCommandValidator : AbstractValidator<VerifyServiceAreaCommand>
{
    public VerifyServiceAreaCommandValidator()
    {
        RuleFor(command => command.ReviewerId).RequiredId("Reviewer id");

        RuleFor(command => command.MasterId).RequiredId("Master id");
    }
}
