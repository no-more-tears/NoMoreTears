using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequestMediaFile;

public sealed class GetRequestMediaFileQueryValidator : AbstractValidator<GetRequestMediaFileQuery>
{
    public GetRequestMediaFileQueryValidator()
    {
        RuleFor(query => query.RequestId).RequiredId("Request id");

        RuleFor(query => query.MediaFileId).RequiredId("Media file id");
    }
}