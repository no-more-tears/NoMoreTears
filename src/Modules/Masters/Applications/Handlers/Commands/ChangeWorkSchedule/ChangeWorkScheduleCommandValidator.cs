using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeWorkSchedule;

public sealed class ChangeWorkScheduleCommandValidator : AbstractValidator<ChangeWorkScheduleCommand>
{
    public ChangeWorkScheduleCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.WorkSchedule).ValidWorkSchedule();
    }
}
