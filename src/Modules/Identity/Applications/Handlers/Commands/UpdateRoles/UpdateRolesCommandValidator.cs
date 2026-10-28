using FluentValidation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.UpdateRoles;

public sealed class UpdateRolesCommandValidator : AbstractValidator<UpdateRolesCommand>
{
    public UpdateRolesCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(command => command.Roles)
            .NotEmpty().WithMessage("At least one role is required.");

        RuleForEach(command => command.Roles)
            .IsInEnum().WithMessage("Role is not supported.");
    }
}
