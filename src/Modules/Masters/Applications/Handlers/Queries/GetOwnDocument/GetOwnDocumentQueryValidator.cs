using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetOwnDocument;

public sealed class GetOwnDocumentQueryValidator : AbstractValidator<GetOwnDocumentQuery>
{
    public GetOwnDocumentQueryValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(query => query.DocumentId).RequiredId("Document id");
    }
}
