using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetMasters;

public sealed class GetMastersQueryValidator : AbstractValidator<GetMastersQuery>
{
    public GetMastersQueryValidator()
    {
        RuleFor(query => query.PageNumber).PageNumber();

        RuleFor(query => query.PageSize).PageSize();
    }
}
