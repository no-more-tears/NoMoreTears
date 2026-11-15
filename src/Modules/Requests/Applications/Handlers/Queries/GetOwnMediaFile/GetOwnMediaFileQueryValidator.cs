using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnMediaFile;

public sealed class GetOwnMediaFileQueryValidator : AbstractValidator<GetOwnMediaFileQuery>
{
    public GetOwnMediaFileQueryValidator()
    {
        RuleFor(query => query.ClientId).RequiredId("Client id");

        RuleFor(query => query.RequestId).RequiredId("Request id");

        RuleFor(query => query.MediaFileId).RequiredId("Media file id");
    }
}