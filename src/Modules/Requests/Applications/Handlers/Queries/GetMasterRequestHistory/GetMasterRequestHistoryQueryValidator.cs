using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterRequestHistory;

public sealed class GetMasterRequestHistoryQueryValidator : AbstractValidator<GetMasterRequestHistoryQuery>
{
    public GetMasterRequestHistoryQueryValidator()
    {
        RuleFor(query => query.MasterId).RequiredId("Master id");

        RuleFor(query => query.RequestId).RequiredId("Request id");
    }
}