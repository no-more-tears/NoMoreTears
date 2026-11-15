using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequestHistory;

public sealed class GetRequestHistoryQueryValidator : AbstractValidator<GetRequestHistoryQuery>
{
    public GetRequestHistoryQueryValidator()
    {
        RuleFor(query => query.RequestId).RequiredId("Request id");
    }
}