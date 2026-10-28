using FluentValidation;
using NoMoreTears.Modules.Identity.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(command => command.Token)
            .Token();
    }
}
