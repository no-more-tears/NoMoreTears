using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.LiftTemporaryMeasure;

public sealed class LiftTemporaryMeasureCommandValidator : AbstractValidator<LiftTemporaryMeasureCommand>
{
    public LiftTemporaryMeasureCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.MeasureId).RequiredId("Measure id");
    }
}
