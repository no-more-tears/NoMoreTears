using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequests;

public sealed class GetRequestsQueryValidator : AbstractValidator<GetRequestsQuery>
{
    public GetRequestsQueryValidator()
    {
        RuleFor(query => query.PageNumber).PageNumber();

        RuleFor(query => query.PageSize).PageSize();
    }
}