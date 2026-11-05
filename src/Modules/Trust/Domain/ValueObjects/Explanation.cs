using NoMoreTears.Modules.Trust.Domain.Exceptions;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Explanation of a temporary measure shown to the user.
/// </summary>
public sealed record Explanation
{
    /// <summary>
    ///     Trimmed text.
    /// </summary>
    public string Value { get; }

    private Explanation(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a value from the given text.
    /// </summary>
    public static Explanation Of(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;

        if (normalized.Length == 0 || normalized.Length > TrustRules.MaxExplanationLength)
        {
            throw new InvalidTextException(
                $"Explanation must contain from 1 to {TrustRules.MaxExplanationLength} characters.");
        }

        return new Explanation(normalized);
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}