using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ApplyTemporaryMeasure;

public sealed class ApplyTemporaryMeasureCommandValidator : AbstractValidator<ApplyTemporaryMeasureCommand>
{
    public ApplyTemporaryMeasureCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.MeasureId).RequiredId("Measure id");

        RuleFor(command => command.Rule).Rule();
    }
}
