using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ProcessMediaUpload;

public sealed class ProcessMediaUploadCommandValidator : AbstractValidator<ProcessMediaUploadCommand>
{
    public ProcessMediaUploadCommandValidator()
    {
        RuleFor(command => command.UploadId).RequiredId("Upload id");
    }
}