using NoMoreTears.Modules.Trust.Domain.Exceptions;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Mandatory justification of a decision of an arbiter,
///     an administrator or a reviewer.
/// </summary>
public sealed record Justification
{
    /// <summary>
    ///     Trimmed text.
    /// </summary>
    public string Value { get; }

    private Justification(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a value from the given text.
    /// </summary>
    public static Justification Of(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;

        if (normalized.Length == 0 || normalized.Length > TrustRules.MaxJustificationLength)
        {
            throw new InvalidTextException(
                $"Justification must contain from 1 to {TrustRules.MaxJustificationLength} characters.");
        }

        return new Justification(normalized);
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}