using FluentValidation;
using NoMoreTears.Shared.CQS.Primitives.Queries;

namespace NoMoreTears.Shared.CQS.Abstractions.Validators;

public abstract class PagedQueryValidator : AbstractValidator<IPagedQuery>
{
    private const int MinValue = 0;
    private const int MaxPageSize = 100;

    protected PagedQueryValidator()
    {
        RuleFor(q => q.PageNumber)
            .GreaterThan(MinValue).WithMessage($"Page number must be greater than {MinValue}.");

        RuleFor(q => q.PageSize)
            .GreaterThan(MinValue).WithMessage($"Page number must be greater than {MinValue}.")
            .LessThanOrEqualTo(MaxPageSize).WithMessage($"Page size must not exceed {MaxPageSize} items.");
    }
}