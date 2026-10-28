using FluentValidation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.LogoutAll;

public sealed class LogoutAllCommandValidator : AbstractValidator<LogoutAllCommand>
{
    public LogoutAllCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage("User id is required.");
    }
}
