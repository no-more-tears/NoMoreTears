using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeFullName;

public sealed class ChangeFullNameCommandValidator : AbstractValidator<ChangeFullNameCommand>
{
    public ChangeFullNameCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.FirstName).NamePart("First name");

        RuleFor(command => command.LastName).NamePart("Last name");
    }
}
