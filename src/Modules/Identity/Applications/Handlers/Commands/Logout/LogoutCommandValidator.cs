using FluentValidation;
using NoMoreTears.Modules.Identity.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Logout;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(command => command.RefreshToken)
            .Token();
    }
}
