using FluentValidation;
using NoMoreTears.Modules.Notification.Domain;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Validation;

/// <summary>
///     Shared validation rules of Notification requests.
/// </summary>
internal static class NotificationValidationRules
{
    public const int MaxBatchSize = 100;

    public static IRuleBuilderOptions<T, Guid> RequiredId<T>(this IRuleBuilder<T, Guid> rule, string name) =>
        rule.NotEmpty().WithMessage($"{name} is required.");

    public static IRuleBuilderOptions<T, IReadOnlyList<Guid>> NotificationIds<T>(this IRuleBuilder<T, IReadOnlyList<Guid>> rule) =>
        rule
            .NotEmpty().WithMessage("At least one notification id is required.")
            .Must(ids => ids is null || ids.Count <= MaxBatchSize)
            .WithMessage($"At most {MaxBatchSize} notifications can be changed at once.");

    public static IRuleBuilderOptions<T, string> Type<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .NotEmpty().WithMessage("Notification type is required.")
            .MaximumLength(NotificationRules.MaxTypeLength)
            .WithMessage($"Notification type must not exceed {NotificationRules.MaxTypeLength} characters.");

    public static IRuleBuilderOptions<T, int> PageNumber<T>(this IRuleBuilder<T, int> rule) =>
        rule.GreaterThan(0).WithMessage("Page number must be greater than 0.");

    public static IRuleBuilderOptions<T, int> PageSize<T>(this IRuleBuilder<T, int> rule) =>
        rule
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100 items.");
}
