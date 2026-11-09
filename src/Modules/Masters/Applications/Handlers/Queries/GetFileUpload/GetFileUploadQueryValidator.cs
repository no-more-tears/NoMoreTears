using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetFileUpload;

public sealed class GetFileUploadQueryValidator : AbstractValidator<GetFileUploadQuery>
{
    public GetFileUploadQueryValidator()
    {
        RuleFor(command => command.MasterId).RequiredId("Master id");

        RuleFor(query => query.UploadId).RequiredId("Upload id");
    }
}
