namespace NoMoreTears.Modules.Dispatch.Domain.ValueObjects;

/// <summary>
///     Score of a candidate for a request.
/// </summary>
public sealed record Score
{
    /// <summary>
    ///     Factors the score is calculated from.
    /// </summary>
    public ScoreFactors Factors { get; }

    /// <summary>
    ///     Whether the score was lowered because the master is restricted.
    /// </summary>
    public bool IsPenalized { get; }

    /// <summary>
    ///     Bonus added because the request was republished.
    /// </summary>
    public double Bonus { get; }

    /// <summary>
    ///     Final score used to order the feed.
    /// </summary>
    public double Total { get; }

    private Score(
        ScoreFactors factors,
        bool isPenalized,
        double bonus,
        double total)
    {
        Factors = factors;
        IsPenalized = isPenalized;
        Bonus = bonus;
        Total = total;
    }

    /// <summary>
    ///     Calculates the score without a bonus.
    /// </summary>
    internal static Score Calculate(
        ScoreFactors factors,
        ScoringWeights weights,
        double penalty)
    {
        return new Score(factors, penalty > 0, 0, weights.Apply(factors) * (1 - penalty));
    }

    /// <summary>
    ///     Returns the score with the republication bonus added.
    /// </summary>
    internal Score WithBonus(double bonus) => new(Factors, IsPenalized, bonus, Total - Bonus + bonus);
}