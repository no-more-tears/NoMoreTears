using FluentValidation;
using NoMoreTears.Modules.Identity.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private const int EmailMaxLength = 256;
    private const int UsernameMinLength = 3;
    private const int UsernameMaxLength = 32;
    private const string UsernamePattern = @"^[a-zA-Z0-9._-]+$";

    public RegisterCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(EmailMaxLength).WithMessage($"Email must not exceed {EmailMaxLength} characters.")
            .EmailAddress().WithMessage("Email is not a valid email address.");

        RuleFor(command => command.Username)
            .MinimumLength(UsernameMinLength)
            .WithMessage($"Username must be at least {UsernameMinLength} characters long.")
            .MaximumLength(UsernameMaxLength)
            .WithMessage($"Username must not exceed {UsernameMaxLength} characters.")
            .Matches(UsernamePattern)
            .WithMessage("Username can only contain letters, digits, dots, underscores and hyphens.")
            .When(command => command.Username is not null);

        RuleFor(command => command.Password)
            .StrongPassword();
    }
}
