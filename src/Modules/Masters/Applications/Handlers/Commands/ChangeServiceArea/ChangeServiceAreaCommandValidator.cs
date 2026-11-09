using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeServiceArea;

public sealed class ChangeServiceAreaCommandValidator : AbstractValidator<ChangeServiceAreaCommand>
{
    public ChangeServiceAreaCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.ServiceArea).ValidServiceArea();
    }
}
