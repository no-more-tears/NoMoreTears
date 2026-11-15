using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMediaUpload;

public sealed class GetMediaUploadQueryValidator : AbstractValidator<GetMediaUploadQuery>
{
    public GetMediaUploadQueryValidator()
    {
        RuleFor(query => query.ClientId).RequiredId("Client id");

        RuleFor(query => query.RequestId).RequiredId("Request id");

        RuleFor(query => query.UploadId).RequiredId("Upload id");
    }
}