using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMasterDocument;

public sealed class GetMasterDocumentQueryValidator : AbstractValidator<GetMasterDocumentQuery>
{
    public GetMasterDocumentQueryValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(query => query.DocumentId).RequiredId("Document id");
    }
}
