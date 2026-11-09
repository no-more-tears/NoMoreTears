using FluentValidation;
using NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetCategory;

public sealed class GetCategoryQueryValidator : AbstractValidator<GetCategoryQuery>
{
    public GetCategoryQueryValidator()
    {
        RuleFor(command => command.CategoryId).RequiredId("Category id");
    }
}
