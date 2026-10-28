using FluentValidation;
using NoMoreTears.Modules.Identity.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.UpdateMfa;

public sealed class UpdateMfaCommandValidator : AbstractValidator<UpdateMfaCommand>
{
    public UpdateMfaCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotEmpty().WithMessage("User id is required.");

        RuleFor(command => command.Password)
            .Password();
    }
}
