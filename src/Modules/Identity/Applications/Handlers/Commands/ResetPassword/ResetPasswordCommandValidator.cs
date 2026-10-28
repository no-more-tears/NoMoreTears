using FluentValidation;
using NoMoreTears.Modules.Identity.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(command => command.Token)
            .Token();

        RuleFor(command => command.NewPassword)
            .StrongPassword();
    }
}
