using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.LiftSanction;

public sealed class LiftSanctionCommandValidator : AbstractValidator<LiftSanctionCommand>
{
    public LiftSanctionCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.SanctionId).RequiredId("Sanction id");
    }
}
