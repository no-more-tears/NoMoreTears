using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.SaveCategorySnapshot;

public sealed class SaveCategorySnapshotCommandValidator : AbstractValidator<SaveCategorySnapshotCommand>
{
    public SaveCategorySnapshotCommandValidator()
    {
        RuleFor(command => command.CategoryId).RequiredId("Category id");

        RuleFor(command => command.Name).NotEmpty().WithMessage("Category name is required.");
    }
}