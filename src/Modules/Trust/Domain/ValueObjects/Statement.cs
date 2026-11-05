using NoMoreTears.Modules.Trust.Domain.Exceptions;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Statement of a participant:
///     a complaint, evidence, an appeal or a contest of a measure.
/// </summary>
public sealed record Statement
{
    /// <summary>
    ///     Trimmed text.
    /// </summary>
    public string Value { get; }

    private Statement(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a value from the given text.
    /// </summary>
    public static Statement Of(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;

        if (normalized.Length == 0 || normalized.Length > TrustRules.MaxStatementLength)
        {
            throw new InvalidTextException(
                $"Statement must contain from 1 to {TrustRules.MaxStatementLength} characters.");
        }

        return new Statement(normalized);
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}