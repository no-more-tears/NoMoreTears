using FluentValidation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.GrantMasterRole;

public sealed class GrantMasterRoleCommandValidator : AbstractValidator<GrantMasterRoleCommand>
{
    public GrantMasterRoleCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage("User id is required.");
    }
}
