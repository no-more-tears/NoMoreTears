using NoMoreTears.Modules.Requests.Domain.Exceptions;

namespace NoMoreTears.Modules.Requests.Domain.ValueObjects;

/// <summary>
///     Geocoded address where the work must be done.
/// </summary>
public sealed record Address
{
    /// <summary>
    ///     Formatted address text.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     Coordinates produced by geocoding.
    /// </summary>
    public Coordinates Coordinates { get; }

    private Address(
        string value,
        Coordinates coordinates)
    {
        Value = value;
        Coordinates = coordinates;
    }

    /// <summary>
    ///     Creates a geocoded address.
    /// </summary>
    public static Address Of(
        string value,
        Coordinates coordinates)
    {
        ArgumentNullException.ThrowIfNull(coordinates);

        var normalized = value?.Trim() ?? string.Empty;

        if (normalized.Length == 0)
        {
            throw new InvalidAddressException("Address must not be empty.");
        }

        if (normalized.Length > RequestRules.MaxAddressLength)
        {
            throw new InvalidAddressException(
                $"Address must not exceed {RequestRules.MaxAddressLength} characters.");
        }

        return new Address(normalized, coordinates);
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}