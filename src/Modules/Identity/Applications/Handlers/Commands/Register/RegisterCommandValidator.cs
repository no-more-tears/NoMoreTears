using FluentValidation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    private const int EmailMaxLength = 256;
    private const int UsernameMinLength = 3;
    private const int UsernameMaxLength = 32;
    private const string UsernamePattern = @"^[a-zA-Z0-9._-]+$";
    private const int PasswordMinLength = 8;
    private const int PasswordMaxLength = 128;
    private const string UppercasePattern = "[A-Z]";
    private const string LowercasePattern = "[a-z]";
    private const string DigitPattern = "[0-9]";
    private const string SpecialCharacterPattern = "[^a-zA-Z0-9]";

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
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(PasswordMinLength)
            .WithMessage($"Password must be at least {PasswordMinLength} characters long.")
            .MaximumLength(PasswordMaxLength)
            .WithMessage($"Password must not exceed {PasswordMaxLength} characters.")
            .Matches(UppercasePattern).WithMessage("Password must contain at least one uppercase letter.")
            .Matches(LowercasePattern).WithMessage("Password must contain at least one lowercase letter.")
            .Matches(DigitPattern).WithMessage("Password must contain at least one digit.")
            .Matches(SpecialCharacterPattern).WithMessage("Password must contain at least one special character.");
    }
}