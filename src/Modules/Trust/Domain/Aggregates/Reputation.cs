using NoMoreTears.Modules.Trust.Domain.Exceptions;
using NoMoreTears.Modules.Trust.Domain.ValueObjects;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Aggregates;

/// <summary>
///     Reputation of a master calculated only from verified reviews and reliability indicators.
/// </summary>
public sealed class Reputation : AuditableEntity<Guid>
{
    /// <summary>
    ///     Rep(m) from 0 to 1: Bayesian average of weighted ratings divided by 5.
    /// </summary>
    public double Score { get; private set; }

    /// <summary>
    ///     Number of verified reviews n.
    /// </summary>
    public int ReviewCount { get; private set; }

    /// <summary>
    ///     Weighted average rating.
    /// </summary>
    public double? AverageRating { get; private set; }

    /// <summary>
    ///     Share of accepted requests completed over 90 days.
    /// </summary>
    public double CompletionRate { get; private set; }

    /// <summary>
    ///     Share of accepted requests that ended with a no-show over 90 days.
    /// </summary>
    public double NoShowRate { get; private set; }

    /// <summary>
    ///     Requests completed over the whole time, shown in the public profile.
    /// </summary>
    public int CompletedRequests { get; private set; }

    /// <summary>
    ///     Moment of the latest calculation.
    /// </summary>
    public DateTimeOffset? CalculatedAt { get; private set; }

    private Reputation(Guid id)
    {
        Id = id;
    }

    /// <summary>
    ///     Creates an empty reputation of a master.
    /// </summary>
    public static Reputation Create(
        Guid masterId,
        DateTimeOffset createdAt)
    {
        if (masterId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(masterId));
        }

        var reputation = new Reputation(masterId);
        reputation.MarkAsCreated(createdAt);

        return reputation;
    }

    /// <summary>
    ///     Recalculates the reputation after a relevant event:
    ///     Rep = ((n / (n + k)) · avg_weighted + (k / (n + k)) · C) / 5, where a review weighs 0.5 ^ (age in days / 180).
    /// </summary>
    public void Recalculate(
        IReadOnlyCollection<Review> reviews,
        double platformAverageRating,
        ReliabilityIndicators reliability,
        DateTimeOffset calculatedAt)
    {
        ArgumentNullException.ThrowIfNull(reviews);
        ArgumentNullException.ThrowIfNull(reliability);

        if (!double.IsFinite(platformAverageRating)
            || platformAverageRating < TrustRules.MinRating
            || platformAverageRating > TrustRules.MaxRating)
        {
            throw new InvalidReputationInputException("Platform average rating must be from 1 to 5.");
        }

        if (reviews.Any(review => review.MasterId != Id))
        {
            throw new InvalidReputationInputException("All reviews must belong to the master.");
        }

        var weightSum = 0.0;
        var weightedRatings = 0.0;
        var count = 0;

        foreach (var review in reviews.Where(review => review.CountsTowardReputation))
        {
            var ageInDays = Math.Max(0, (calculatedAt - (review.CreatedAt ?? calculatedAt)).TotalDays);
            var weight = review.Weight * Math.Pow(0.5, ageInDays / TrustRules.ReviewHalfLifeInDays);

            weightSum += weight;
            weightedRatings += weight * review.Rating.Value;
            count++;
        }

        var average = weightSum > 0 ? weightedRatings / weightSum : platformAverageRating;
        var evidenceShare = (double)count / (count + TrustRules.ReputationPrior);

        Score = ((evidenceShare * average) + ((1 - evidenceShare) * platformAverageRating)) / TrustRules.MaxRating;
        ReviewCount = count;
        AverageRating = count > 0 ? average : null;
        CompletionRate = reliability.CompletionRate;
        NoShowRate = reliability.NoShowRate;
        CompletedRequests = reliability.CompletedTotal;
        CalculatedAt = calculatedAt;

        MarkAsUpdated(calculatedAt);
    }
}