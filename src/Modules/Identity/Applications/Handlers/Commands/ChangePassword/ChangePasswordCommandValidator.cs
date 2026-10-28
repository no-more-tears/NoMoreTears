using FluentValidation;
using NoMoreTears.Modules.Identity.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ChangePassword;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(command => command.CurrentPassword)
            .Password();

        RuleFor(command => command.NewPassword)
            .StrongPassword()
            .NotEqual(command => command.CurrentPassword)
            .WithMessage("New password must differ from the current password.");
    }
}
