using NoMoreTears.Modules.Requests.Domain.Exceptions;

namespace NoMoreTears.Modules.Requests.Domain.ValueObjects;

/// <summary>
///     Terms under which a client publishes requests.
/// </summary>
public sealed record PublicationPolicy
{
    /// <summary>
    ///     Default terms: 24 hours of waiting and 3 published requests.
    /// </summary>
    public static PublicationPolicy Default { get; } = new(
        RequestRules.DefaultAcceptanceWaitingPeriod,
        RequestRules.DefaultPublishedRequestsLimit);

    /// <summary>
    ///     Period during which a published request waits for acceptance.
    /// </summary>
    public TimeSpan AcceptanceWaitingPeriod { get; }

    /// <summary>
    ///     Maximum number of requests a client can have published at the same time.
    /// </summary>
    public int PublishedRequestsLimit { get; }

    private PublicationPolicy(
        TimeSpan acceptanceWaitingPeriod,
        int publishedRequestsLimit)
    {
        AcceptanceWaitingPeriod = acceptanceWaitingPeriod;
        PublishedRequestsLimit = publishedRequestsLimit;
    }

    /// <summary>
    ///     Creates publication terms.
    /// </summary>
    public static PublicationPolicy Of(
        TimeSpan acceptanceWaitingPeriod,
        int publishedRequestsLimit)
    {
        if (acceptanceWaitingPeriod <= TimeSpan.Zero)
        {
            throw new InvalidPublicationPolicyException("Acceptance waiting period must be positive.");
        }

        if (publishedRequestsLimit < 1)
        {
            throw new InvalidPublicationPolicyException("Published requests limit must be at least 1.");
        }

        return new PublicationPolicy(acceptanceWaitingPeriod, publishedRequestsLimit);
    }

    /// <summary>
    ///     Ensures that one more request can be published.
    /// </summary>
    internal void EnsureAllowsPublication(int publishedRequestsCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(publishedRequestsCount);

        if (publishedRequestsCount >= PublishedRequestsLimit)
        {
            throw new PublicationLimitExceededException(PublishedRequestsLimit);
        }
    }

    /// <summary>
    ///     Calculates the acceptance deadline for a publication made at the given moment.
    /// </summary>
    internal DateTimeOffset AcceptanceDeadlineFrom(DateTimeOffset publishedAt) => publishedAt + AcceptanceWaitingPeriod;
}