using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequest;

public sealed class GetOwnRequestQueryValidator : AbstractValidator<GetOwnRequestQuery>
{
    public GetOwnRequestQueryValidator()
    {
        RuleFor(query => query.ClientId).RequiredId("Client id");

        RuleFor(query => query.RequestId).RequiredId("Request id");
    }
}