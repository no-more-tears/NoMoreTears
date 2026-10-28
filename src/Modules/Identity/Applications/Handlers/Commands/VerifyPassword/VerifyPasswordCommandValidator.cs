using FluentValidation;
using NoMoreTears.Modules.Identity.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.VerifyPassword;

public sealed class VerifyPasswordCommandValidator : AbstractValidator<VerifyPasswordCommand>
{
    public VerifyPasswordCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(command => command.Password)
            .Password();
    }
}
