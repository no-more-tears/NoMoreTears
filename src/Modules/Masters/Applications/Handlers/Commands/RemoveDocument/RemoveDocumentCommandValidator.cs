using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RemoveDocument;

public sealed class RemoveDocumentCommandValidator : AbstractValidator<RemoveDocumentCommand>
{
    public RemoveDocumentCommandValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(command => command.DocumentId).RequiredId("Document id");
    }
}
