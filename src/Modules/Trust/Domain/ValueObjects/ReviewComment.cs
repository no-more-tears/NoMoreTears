using NoMoreTears.Modules.Trust.Domain.Exceptions;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Text of a review left by a client.
/// </summary>
public sealed record ReviewComment
{
    /// <summary>
    ///     Trimmed text.
    /// </summary>
    public string Value { get; }

    private ReviewComment(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a value from the given text.
    /// </summary>
    public static ReviewComment Of(string value)
    {
        var normalized = value?.Trim() ?? string.Empty;

        if (normalized.Length == 0 || normalized.Length > TrustRules.MaxReviewCommentLength)
        {
            throw new InvalidTextException(
                $"Review comment must contain from 1 to {TrustRules.MaxReviewCommentLength} characters.");
        }

        return new ReviewComment(normalized);
    }

    /// <inheritdoc />
    public override string ToString() => Value;
}