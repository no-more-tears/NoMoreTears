using FluentValidation;
using NoMoreTears.Modules.Identity.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(command => command.Identifier)
            .Identifier();
    }
}
