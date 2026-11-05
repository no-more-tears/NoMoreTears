using NoMoreTears.Modules.Masters.Domain.Exceptions;

namespace NoMoreTears.Modules.Masters.Domain.ValueObjects;

/// <summary>
///     Reason of a review decision or of a verification hold.
/// </summary>
public sealed record Reason
{
    /// <summary>
    ///     Trimmed reason text.
    /// </summary>
    public string Value { get; }

    private Reason(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a reason from the given text.
    /// </summary>
    public static Reason Of(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;

        if (normalized.Length == 0)
        {
            throw new InvalidReasonException("Reason must not be empty.");
        }

        if (normalized.Length > MasterRules.MaxReasonLength)
        {
            throw new InvalidReasonException(
                $"Reason must not exceed {MasterRules.MaxReasonLength} characters.");
        }

        return new Reason(normalized);
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}