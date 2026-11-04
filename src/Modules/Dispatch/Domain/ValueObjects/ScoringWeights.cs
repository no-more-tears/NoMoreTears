using NoMoreTears.Modules.Dispatch.Domain.Exceptions;

namespace NoMoreTears.Modules.Dispatch.Domain.ValueObjects;

/// <summary>
///     Weights of the scoring factors; they sum to 1.
/// </summary>
public sealed record ScoringWeights
{
    /// <summary>
    ///     Default weights: 0.35, 0.25, 0.25, 0.15.
    /// </summary>
    public static ScoringWeights Default { get; } = new(0.35, 0.25, 0.25, 0.15);

    /// <summary>
    ///     Weight of reputation.
    /// </summary>
    public double Reputation { get; }

    /// <summary>
    ///     Weight of proximity.
    /// </summary>
    public double Proximity { get; }

    /// <summary>
    ///     Weight of reliability.
    /// </summary>
    public double Reliability { get; }

    /// <summary>
    ///     Weight of fair load.
    /// </summary>
    public double Fairness { get; }

    private ScoringWeights(
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
    ///     Creates scoring weights.
    /// </summary>
    public static ScoringWeights Of(
        double reputation,
        double proximity,
        double reliability,
        double fairness)
    {
        double[] weights = [reputation, proximity, reliability, fairness];

        if (Array.Exists(weights, weight => !double.IsFinite(weight) || weight < 0))
        {
            throw new InvalidPolicyException("Scoring weights must be non-negative numbers.");
        }

        if (Math.Abs(weights.Sum() - 1) > DispatchRules.Tolerance)
        {
            throw new InvalidPolicyException("Scoring weights must sum to 1.");
        }

        return new ScoringWeights(reputation, proximity, reliability, fairness);
    }

    /// <summary>
    ///     Calculates the weighted sum of the factors.
    /// </summary>
    public double Apply(ScoreFactors factors)
    {
        ArgumentNullException.ThrowIfNull(factors);

        return (Reputation * factors.Reputation)
            + (Proximity * factors.Proximity)
            + (Reliability * factors.Reliability)
            + (Fairness * factors.Fairness);
    }
}