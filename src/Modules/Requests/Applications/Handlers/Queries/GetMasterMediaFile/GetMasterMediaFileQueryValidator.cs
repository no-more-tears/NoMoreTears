using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterMediaFile;

public sealed class GetMasterMediaFileQueryValidator : AbstractValidator<GetMasterMediaFileQuery>
{
    public GetMasterMediaFileQueryValidator()
    {
        RuleFor(query => query.MasterId).RequiredId("Master id");

        RuleFor(query => query.RequestId).RequiredId("Request id");

        RuleFor(query => query.MediaFileId).RequiredId("Media file id");
    }
}