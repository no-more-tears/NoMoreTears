using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequest;

public sealed class GetRequestQueryValidator : AbstractValidator<GetRequestQuery>
{
    public GetRequestQueryValidator()
    {
        RuleFor(query => query.RequestId).RequiredId("Request id");
    }
}