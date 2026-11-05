using NoMoreTears.Modules.Trust.Domain.Exceptions;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Rating of a review from 1 to 5.
/// </summary>
public sealed record Rating
{
    /// <summary>
    ///     Rating value.
    /// </summary>
    public int Value { get; }

    /// <summary>
    ///     Whether the rating is the highest one.
    /// </summary>
    public bool IsHighest => Value == TrustRules.MaxRating;

    private Rating(int value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a rating.
    /// </summary>
    public static Rating Of(int value)
    {
        if (value is < TrustRules.MinRating or > TrustRules.MaxRating)
        {
            throw new InvalidRatingException(value);
        }

        return new Rating(value);
    }
}