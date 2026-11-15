using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequestHistory;

public sealed class GetOwnRequestHistoryQueryValidator : AbstractValidator<GetOwnRequestHistoryQuery>
{
    public GetOwnRequestHistoryQueryValidator()
    {
        RuleFor(query => query.ClientId).RequiredId("Client id");

        RuleFor(query => query.RequestId).RequiredId("Request id");
    }
}