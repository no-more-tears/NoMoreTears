using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ArchiveCategorySnapshot;

public sealed class ArchiveCategorySnapshotCommandValidator : AbstractValidator<ArchiveCategorySnapshotCommand>
{
    public ArchiveCategorySnapshotCommandValidator()
    {
        RuleFor(command => command.CategoryId).RequiredId("Category id");
    }
}