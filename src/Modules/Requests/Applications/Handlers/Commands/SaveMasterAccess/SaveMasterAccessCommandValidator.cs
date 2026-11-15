using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.SaveMasterAccess;

public sealed class SaveMasterAccessCommandValidator : AbstractValidator<SaveMasterAccessCommand>
{
    public SaveMasterAccessCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.AcceptableCategoryIds).NotNull().WithMessage("Categories are required.");
    }
}