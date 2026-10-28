using FluentValidation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.RevokeSession;

public sealed class RevokeSessionCommandValidator : AbstractValidator<RevokeSessionCommand>
{
    public RevokeSessionCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(command => command.SessionId)
            .NotEmpty().WithMessage("Session id is required.");
    }
}
