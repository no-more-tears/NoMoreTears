using NoMoreTears.Modules.Masters.Domain.Exceptions;

namespace NoMoreTears.Modules.Masters.Domain.ValueObjects;

/// <summary>
///     Decision of a PlatformSupport reviewer on a master,
///     a category or a service area.
/// </summary>
public sealed record ReviewDecision
{
    /// <summary>
    ///     Identifier of the reviewer.
    /// </summary>
    public Guid ReviewerId { get; }

    /// <summary>
    ///     Moment of the decision.
    /// </summary>
    public DateTimeOffset ReviewedAt { get; }

    /// <summary>
    ///     Reason of a rejection.
    /// </summary>
    public Reason? Reason { get; }

    private ReviewDecision(
        Guid reviewerId,
        DateTimeOffset reviewedAt,
        Reason? reason)
    {
        ReviewerId = reviewerId;
        ReviewedAt = reviewedAt;
        Reason = reason;
    }

    /// <summary>
    ///     Creates a review decision.
    /// </summary>
    public static ReviewDecision Of(
        Guid reviewerId,
        DateTimeOffset reviewedAt,
        Reason? reason)
    {
        if (reviewerId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(reviewerId));
        }

        return new ReviewDecision(reviewerId, reviewedAt, reason);
    }
}