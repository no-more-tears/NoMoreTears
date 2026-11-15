using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ActivateCategorySnapshot;

public sealed class ActivateCategorySnapshotCommandValidator : AbstractValidator<ActivateCategorySnapshotCommand>
{
    public ActivateCategorySnapshotCommandValidator()
    {
        RuleFor(command => command.CategoryId).RequiredId("Category id");
    }
}