using FluentValidation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Validation;

/// <summary>
///     Validation rules shared by identity commands.
/// </summary>
public static class IdentityValidationRules
{
    public const int IdentifierMaxLength = 256;
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 128;
    public const int TokenMaxLength = 2048;

    private const string UppercasePattern = "[A-Z]";
    private const string LowercasePattern = "[a-z]";
    private const string DigitPattern = "[0-9]";
    private const string SpecialCharacterPattern = "[^a-zA-Z0-9]";

    /// <summary>
    ///     Email address or username used to look up an account.
    /// </summary>
    public static IRuleBuilderOptions<T, string> Identifier<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("Email or username is required.")
            .MaximumLength(IdentifierMaxLength)
            .WithMessage($"Email or username must not exceed {IdentifierMaxLength} characters.");

    /// <summary>
    ///     Existing password presented for authentication.
    /// </summary>
    public static IRuleBuilderOptions<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(PasswordMaxLength)
            .WithMessage($"{{PropertyName}} must not exceed {PasswordMaxLength} characters.");

    /// <summary>
    ///     Password that is about to be set and has to satisfy the complexity policy.
    /// </summary>
    public static IRuleBuilderOptions<T, string> StrongPassword<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MinimumLength(PasswordMinLength)
            .WithMessage($"{{PropertyName}} must be at least {PasswordMinLength} characters long.")
            .MaximumLength(PasswordMaxLength)
            .WithMessage($"{{PropertyName}} must not exceed {PasswordMaxLength} characters.")
            .Matches(UppercasePattern).WithMessage("{PropertyName} must contain at least one uppercase letter.")
            .Matches(LowercasePattern).WithMessage("{PropertyName} must contain at least one lowercase letter.")
            .Matches(DigitPattern).WithMessage("{PropertyName} must contain at least one digit.")
            .Matches(SpecialCharacterPattern).WithMessage("{PropertyName} must contain at least one special character.");

    /// <summary>
    ///     Opaque token issued by the identity provider.
    /// </summary>
    public static IRuleBuilderOptions<T, string> Token<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .MaximumLength(TokenMaxLength)
            .WithMessage($"{{PropertyName}} must not exceed {TokenMaxLength} characters.");
}
