using NoMoreTears.Modules.Requests.Domain.Exceptions;

namespace NoMoreTears.Modules.Requests.Domain.ValueObjects;

/// <summary>
///     Client's description of the problem.
/// </summary>
public sealed record Description
{
    /// <summary>
    ///     Trimmed description text.
    /// </summary>
    public string Value { get; }

    private Description(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a description from the given text.
    /// </summary>
    public static Description Of(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;

        if (normalized.Length < RequestRules.MinDescriptionLength)
        {
            throw new InvalidDescriptionException(
                $"Description must contain at least {RequestRules.MinDescriptionLength} characters.");
        }

        if (normalized.Length > RequestRules.MaxDescriptionLength)
        {
            throw new InvalidDescriptionException(
                $"Description must not exceed {RequestRules.MaxDescriptionLength} characters.");
        }

        return new Description(normalized);
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}