using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DeleteObsoleteFiles;

public sealed class DeleteObsoleteFilesCommandValidator : AbstractValidator<DeleteObsoleteFilesCommand>
{
    public DeleteObsoleteFilesCommandValidator()
    {
        RuleFor(command => command.Files).NotNull().WithMessage("Files are required.");
    }
}