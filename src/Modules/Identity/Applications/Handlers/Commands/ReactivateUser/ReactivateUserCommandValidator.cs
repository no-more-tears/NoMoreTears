using FluentValidation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ReactivateUser;

public sealed class ReactivateUserCommandValidator : AbstractValidator<ReactivateUserCommand>
{
    public ReactivateUserCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage("User id is required.");
    }
}
