using NoMoreTears.Modules.Dispatch.Domain.Exceptions;

namespace NoMoreTears.Modules.Dispatch.Domain.ValueObjects;

/// <summary>
///     Normalized scoring factors of a candidate, each from 0 to 1.
///     They are also the explanation shown to the master.
/// </summary>
public sealed record ScoreFactors
{
    /// <summary>
    ///     Rep(m): reputation of the master.
    /// </summary>
    public double Reputation { get; }

    /// <summary>
    ///     Prox(m, r) = max(0, 1 − distance / radius).
    /// </summary>
    public double Proximity { get; }

    /// <summary>
    ///     Rel(m) = completion rate · (1 − no-show rate).
    /// </summary>
    public double Reliability { get; }

    /// <summary>
    ///     Fair(m) = 1 − min(1, accepted over 7 days / cohort median).
    /// </summary>
    public double Fairness { get; }

    private ScoreFactors(
        double reputation,
        double proximity,
        double reliability,
        double fairness)
    {
        Reputation = reputation;
        Proximity = proximity;
        Reliability = reliability;
        Fairness = fairness;
    }

    /// <summary>
    ///     Creates scoring factors.
    /// </summary>
    public static ScoreFactors Of(
        double reputation,
        double proximity,
        double reliability,
        double fairness)
    {
        double[] factors = [reputation, proximity, reliability, fairness];

        if (Array.Exists(factors, factor => !double.IsFinite(factor) || factor < 0 || factor > 1))
        {
            throw new InvalidScoringDataException("Scoring factors must be from 0 to 1.");
        }

        return new ScoreFactors(reputation, proximity, reliability, fairness);
    }

    /// <summary>
    ///     Calculates the factors of a candidate within a cohort.
    /// </summary>
    internal static ScoreFactors Calculate(
        CandidateProfile profile,
        double cohortMedianAcceptedLastWeek)
    {
        var performance = profile.Performance;
        var proximity = Math.Max(0, 1 - (profile.DistanceInKilometers / profile.ServiceRadiusInKilometers));
        var fairness = cohortMedianAcceptedLastWeek > 0
            ? 1 - Math.Min(1, performance.AcceptedLastWeek / cohortMedianAcceptedLastWeek)
            : performance.AcceptedLastWeek == 0 ? 1 : 0;

        return Of(performance.Reputation, proximity, performance.Reliability, fairness);
    }
}