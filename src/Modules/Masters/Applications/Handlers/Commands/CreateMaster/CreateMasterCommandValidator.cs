using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.CreateMaster;

public sealed class CreateMasterCommandValidator : AbstractValidator<CreateMasterCommand>
{
    public CreateMasterCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.FirstName).NamePart("First name");

        RuleFor(command => command.LastName).NamePart("Last name");

        RuleFor(command => command.CategoryIds)
            .NotEmpty().WithMessage("At least one category is required.");

        RuleForEach(command => command.CategoryIds)
            .NotEmpty().WithMessage("Category id is required.");

        RuleFor(command => command.ServiceArea).ValidServiceArea();

        RuleFor(command => command.WorkSchedule).ValidWorkSchedule();
    }
}
