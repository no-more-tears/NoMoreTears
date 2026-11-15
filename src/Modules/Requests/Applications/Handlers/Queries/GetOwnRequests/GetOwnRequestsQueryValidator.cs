using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequests;

public sealed class GetOwnRequestsQueryValidator : AbstractValidator<GetOwnRequestsQuery>
{
    public GetOwnRequestsQueryValidator()
    {
        RuleFor(query => query.ClientId).RequiredId("Client id");

        RuleFor(query => query.PageNumber).PageNumber();

        RuleFor(query => query.PageSize).PageSize();
    }
}