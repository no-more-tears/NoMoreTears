using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;
using NoMoreTears.Modules.Masters.Domain;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.UploadDocument;

public sealed class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    public UploadDocumentCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.Kind).NotEmpty().WithMessage("Document kind is required.");

        RuleFor(command => command.Content).NotNull().WithMessage("File is required.");

        RuleFor(command => command.FileName).FileName();

        RuleFor(command => command.ContentType).ContentType();

        RuleFor(command => command.SizeInBytes).FileSize(MasterRules.MaxDocumentSizeInBytes);
    }
}
