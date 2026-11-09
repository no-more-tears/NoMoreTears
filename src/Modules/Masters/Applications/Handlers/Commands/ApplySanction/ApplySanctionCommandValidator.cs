using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ApplySanction;

public sealed class ApplySanctionCommandValidator : AbstractValidator<ApplySanctionCommand>
{
    public ApplySanctionCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.SanctionId).RequiredId("Sanction id");

        RuleFor(command => command.Kind).NotEmpty().WithMessage("Sanction kind is required.");
    }
}
