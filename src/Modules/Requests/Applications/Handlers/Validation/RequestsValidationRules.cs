using FluentValidation;
using NoMoreTears.Modules.Requests.Contracts.Requests;
using NoMoreTears.Modules.Requests.Domain;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Validation;

/// <summary>
///     Shared validation rules of Requests requests. The domain enforces the business rules again;
///     validators only reject malformed input early with 400.
/// </summary>
internal static class RequestsValidationRules
{
    /// <summary>
    ///     Arrival intervals a master can choose when accepting a request (FR-DSP-05).
    /// </summary>
    private static readonly int[] ArrivalIntervalsInHours = [2, 6, 24];

    public static IRuleBuilderOptions<T, Guid> RequiredId<T>(this IRuleBuilder<T, Guid> rule, string name) =>
        rule.NotEmpty().WithMessage($"{name} is required.");

    public static IRuleBuilderOptions<T, string> ValidDescription<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .NotEmpty().WithMessage("Description is required.")
            .Must(description => description is null || description.Trim().Length >= RequestRules.MinDescriptionLength)
            .WithMessage($"Description must contain at least {RequestRules.MinDescriptionLength} characters.")
            .Must(description => description is null || description.Trim().Length <= RequestRules.MaxDescriptionLength)
            .WithMessage($"Description must not exceed {RequestRules.MaxDescriptionLength} characters.");

    public static IRuleBuilderOptions<T, AddressRequest> ValidAddress<T>(this IRuleBuilder<T, AddressRequest> rule) =>
        rule
            .NotNull().WithMessage("Address is required.")
            .Must(address => address is null || !string.IsNullOrWhiteSpace(address.Value))
            .WithMessage("Address must not be empty.")
            .Must(address => address?.Value is null || address.Value.Trim().Length <= RequestRules.MaxAddressLength)
            .WithMessage($"Address must not exceed {RequestRules.MaxAddressLength} characters.")
            .Must(address => address is null || address.Coordinates is not null)
            .WithMessage("Coordinates of the address are required.")
            .Must(address => address?.Coordinates is null || address.Coordinates.Latitude is >= -90 and <= 90)
            .WithMessage("Latitude must be between -90 and 90 degrees.")
            .Must(address => address?.Coordinates is null || address.Coordinates.Longitude is >= -180 and <= 180)
            .WithMessage("Longitude must be between -180 and 180 degrees.");

    public static IRuleBuilderOptions<T, string> RequiredReason<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(RequestRules.MaxReasonLength)
            .WithMessage($"Reason must not exceed {RequestRules.MaxReasonLength} characters.");

    public static IRuleBuilderOptions<T, string?> OptionalReason<T>(this IRuleBuilder<T, string?> rule) =>
        rule
            .MaximumLength(RequestRules.MaxReasonLength)
            .WithMessage($"Reason must not exceed {RequestRules.MaxReasonLength} characters.");

    public static IRuleBuilderOptions<T, int> ArrivalInterval<T>(this IRuleBuilder<T, int> rule) =>
        rule
            .Must(hours => Array.IndexOf(ArrivalIntervalsInHours, hours) >= 0)
            .WithMessage("Arrival interval must be 2, 6 or 24 hours.");

    public static IRuleBuilderOptions<T, string> FileName<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .NotEmpty().WithMessage("File name is required.")
            .MaximumLength(RequestRules.MaxFileNameLength)
            .WithMessage($"File name must not exceed {RequestRules.MaxFileNameLength} characters.");

    public static IRuleBuilderOptions<T, string> ContentType<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("Content type is required.");

    public static IRuleBuilderOptions<T, long> FileSize<T>(this IRuleBuilder<T, long> rule) =>
        rule
            .GreaterThan(0).WithMessage("File is empty.")
            .LessThanOrEqualTo(RequestRules.MaxMediaFileSizeInBytes)
            .WithMessage($"File size must not exceed {RequestRules.MaxMediaFileSizeInBytes} bytes.");

    public static IRuleBuilderOptions<T, int> PageNumber<T>(this IRuleBuilder<T, int> rule) =>
        rule.GreaterThan(0).WithMessage("Page number must be greater than 0.");

    public static IRuleBuilderOptions<T, int> PageSize<T>(this IRuleBuilder<T, int> rule) =>
        rule
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Page size must not exceed 100 items.");
}