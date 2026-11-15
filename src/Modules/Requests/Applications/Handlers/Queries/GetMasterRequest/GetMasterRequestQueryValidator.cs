using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterRequest;

public sealed class GetMasterRequestQueryValidator : AbstractValidator<GetMasterRequestQuery>
{
    public GetMasterRequestQueryValidator()
    {
        RuleFor(query => query.MasterId).RequiredId("Master id");

        RuleFor(query => query.RequestId).RequiredId("Request id");
    }
}