using FluentValidation;
using NoMoreTears.Modules.Identity.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.VerifyMfa;

public sealed class VerifyMfaCommandValidator : AbstractValidator<VerifyMfaCommand>
{
    private const int CodeMaxLength = 16;
    private const string CodePattern = "^[0-9]+$";

    public VerifyMfaCommandValidator()
    {
        RuleFor(command => command.ChallengeToken)
            .Token();

        RuleFor(command => command.Code)
            .NotEmpty().WithMessage("Verification code is required.")
            .MaximumLength(CodeMaxLength)
            .WithMessage($"Verification code must not exceed {CodeMaxLength} characters.")
            .Matches(CodePattern).WithMessage("Verification code can only contain digits.");
    }
}
