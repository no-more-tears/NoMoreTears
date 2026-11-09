using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ProcessFileUpload;

public sealed class ProcessFileUploadCommandValidator : AbstractValidator<ProcessFileUploadCommand>
{
    public ProcessFileUploadCommandValidator()
    {
        RuleFor(command => command.UploadId).RequiredId("Upload id");
    }
}
