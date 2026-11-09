using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeAvailability;

public sealed class ChangeAvailabilityCommandValidator : AbstractValidator<ChangeAvailabilityCommand>
{
    public ChangeAvailabilityCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");
    }
}
