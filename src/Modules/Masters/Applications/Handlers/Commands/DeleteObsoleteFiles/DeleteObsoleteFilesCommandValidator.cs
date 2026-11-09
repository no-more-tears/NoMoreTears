using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.DeleteObsoleteFiles;

public sealed class DeleteObsoleteFilesCommandValidator : AbstractValidator<DeleteObsoleteFilesCommand>
{
    public DeleteObsoleteFilesCommandValidator()
    {
        RuleFor(command => command.Files).NotNull().WithMessage("Files are required.");
    }
}
