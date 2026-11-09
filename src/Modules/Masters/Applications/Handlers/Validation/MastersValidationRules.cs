using FluentValidation;
using NoMoreTears.Modules.Masters.Contracts.Requests;
using NoMoreTears.Modules.Masters.Domain;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Validation;

/// <summary>
///     Shared validation rules of Masters requests. The domain enforces the business rules again;
///     validators only reject malformed input early with 400.
/// </summary>
internal static class MastersValidationRules
{
    private const int MaxTimeZoneLength = 64;
    private const int MaxRuleLength = 100;

    public static IRuleBuilderOptions<T, Guid> RequiredId<T>(this IRuleBuilder<T, Guid> rule, string name) =>
        rule.NotEmpty().WithMessage($"{name} is required.");

    public static IRuleBuilderOptions<T, string> RequiredReason<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(MasterRules.MaxReasonLength)
            .WithMessage($"Reason must not exceed {MasterRules.MaxReasonLength} characters.");

    public static IRuleBuilderOptions<T, string> NamePart<T>(this IRuleBuilder<T, string> rule, string name) =>
        rule
            .NotEmpty().WithMessage($"{name} is required.")
            .MaximumLength(MasterRules.MaxNamePartLength)
            .WithMessage($"{name} must not exceed {MasterRules.MaxNamePartLength} characters.");

    public static IRuleBuilderOptions<T, string> CategoryName<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(MasterRules.MaxCategoryNameLength)
            .WithMessage($"Category name must not exceed {MasterRules.MaxCategoryNameLength} characters.");

    public static IRuleBuilderOptions<T, string?> CategoryDescription<T>(this IRuleBuilder<T, string?> rule) =>
        rule
            .MaximumLength(MasterRules.MaxCategoryDescriptionLength)
            .WithMessage($"Category description must not exceed {MasterRules.MaxCategoryDescriptionLength} characters.");

    public static IRuleBuilderOptions<T, string> Rule<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .NotEmpty().WithMessage("Rule is required.")
            .MaximumLength(MaxRuleLength).WithMessage($"Rule must not exceed {MaxRuleLength} characters.");

    public static IRuleBuilderOptions<T, ServiceAreaRequest> ValidServiceArea<T>(this IRuleBuilder<T, ServiceAreaRequest> rule) =>
        rule
            .NotNull().WithMessage("Service area is required.")
            .Must(area => area is null || (area.Center is not null) != (area.Boundary is { Count: > 0 }))
            .WithMessage("Service area must be either a circle (center and radius) or a polygon.");

    public static IRuleBuilderOptions<T, WorkScheduleRequest> ValidWorkSchedule<T>(this IRuleBuilder<T, WorkScheduleRequest> rule) =>
        rule
            .NotNull().WithMessage("Work schedule is required.")
            .Must(schedule => schedule is null
                || (!string.IsNullOrWhiteSpace(schedule.TimeZoneId) && schedule.TimeZoneId.Length <= MaxTimeZoneLength))
            .WithMessage("Time zone of the work schedule is required.")
            .Must(schedule => schedule is null || schedule.Hours is { Count: > 0 })
            .WithMessage("Work schedule must contain at least one working interval.");

    public static IRuleBuilderOptions<T, string> FileName<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .NotEmpty().WithMessage("File name is required.")
            .MaximumLength(MasterRules.MaxFileNameLength)
            .WithMessage($"File name must not exceed {MasterRules.MaxFileNameLength} characters.");

    public static IRuleBuilderOptions<T, string> ContentType<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("Content type is required.");

    public static IRuleBuilderOptions<T, long> FileSize<T>(this IRuleBuilder<T, long> rule, long maxSize) =>
        rule
            .GreaterThan(0).WithMessage("File is empty.")
            .LessThanOrEqualTo(maxSize).WithMessage($"File size must not exceed {maxSize} bytes.");

    public static IRuleBuilderOptions<T, int> PageNumber<T>(this IRuleBuilder<T, int> rule) =>
        rule.GreaterThan(0).WithMessage("Page number must be greater than 0.");

    public static IRuleBuilderOptions<T, int> PageSize<T>(this IRuleBuilder<T, int> rule) =>
        rule
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100 items.");
}
