using FluentValidation;
using NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterRequests;

public sealed class GetMasterRequestsQueryValidator : AbstractValidator<GetMasterRequestsQuery>
{
    public GetMasterRequestsQueryValidator()
    {
        RuleFor(query => query.MasterId).RequiredId("Master id");

        RuleFor(query => query.PageNumber).PageNumber();

        RuleFor(query => query.PageSize).PageSize();
    }
}