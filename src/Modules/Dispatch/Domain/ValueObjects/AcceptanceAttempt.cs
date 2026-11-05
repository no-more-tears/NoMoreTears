namespace NoMoreTears.Modules.Dispatch.Domain.ValueObjects;

/// <summary>
///     Attempt of a master to accept a request,
///     with the reaction time since the request appeared in the feed.
/// </summary>
public sealed record AcceptanceAttempt
{
    /// <summary>
    ///     Moment of the attempt.
    /// </summary>
    public DateTimeOffset AttemptedAt { get; }

    /// <summary>
    ///     Time between the offer and the attempt.
    /// </summary>
    public TimeSpan ReactionTime { get; }

    private AcceptanceAttempt(
        DateTimeOffset attemptedAt, 
        TimeSpan reactionTime)
    {
        AttemptedAt = attemptedAt;
        ReactionTime = reactionTime;
    }

    /// <summary>
    ///     Creates an acceptance attempt.
    /// </summary>
    public static AcceptanceAttempt Of(
        DateTimeOffset attemptedAt,
        TimeSpan reactionTime)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(reactionTime, TimeSpan.Zero);

        return new AcceptanceAttempt(attemptedAt, reactionTime);
    }
}