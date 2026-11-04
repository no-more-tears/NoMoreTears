using NoMoreTears.Modules.Dispatch.Domain.Exceptions;

namespace NoMoreTears.Modules.Dispatch.Domain.ValueObjects;

/// <summary>
///     Indicators of a master received from Trust and Requests and used for scoring.
/// </summary>
public sealed record Performance
{
    /// <summary>
    ///     Indicators of a master without history.
    /// </summary>
    public static Performance Initial { get; } = new(0, 0, 0, 0, 0);

    /// <summary>
    ///     Reputation from Trust, from 0 to 1.
    /// </summary>
    public double Reputation { get; }

    /// <summary>
    ///     Share of completed requests over 90 days, from 0 to 1.
    /// </summary>
    public double CompletionRate { get; }

    /// <summary>
    ///     Share of no-shows over 90 days, from 0 to 1.
    /// </summary>
    public double NoShowRate { get; }

    /// <summary>
    ///     Total number of completed requests.
    /// </summary>
    public int CompletedRequests { get; }

    /// <summary>
    ///     Number of requests accepted over the last 7 days.
    /// </summary>
    public int AcceptedLastWeek { get; }

    /// <summary>
    ///     Reliability factor: completion rate multiplied by the share of arrivals.
    /// </summary>
    public double Reliability => CompletionRate * (1 - NoShowRate);

    private Performance(
        double reputation,
        double completionRate,
        double noShowRate,
        int completedRequests,
        int acceptedLastWeek)
    {
        Reputation = reputation;
        CompletionRate = completionRate;
        NoShowRate = noShowRate;
        CompletedRequests = completedRequests;
        AcceptedLastWeek = acceptedLastWeek;
    }

    /// <summary>
    ///     Creates master indicators.
    /// </summary>
    public static Performance Of(
        double reputation,
        double completionRate,
        double noShowRate,
        int completedRequests,
        int acceptedLastWeek)
    {
        EnsureShare(reputation, nameof(reputation));
        EnsureShare(completionRate, nameof(completionRate));
        EnsureShare(noShowRate, nameof(noShowRate));

        if (completedRequests < 0 || acceptedLastWeek < 0)
        {
            throw new InvalidScoringDataException("Request counters must not be negative.");
        }

        return new Performance(reputation, completionRate, noShowRate, completedRequests, acceptedLastWeek);
    }

    private static void EnsureShare(double value, string name)
    {
        if (!double.IsFinite(value) || value < 0 || value > 1)
        {
            throw new InvalidScoringDataException($"'{name}' must be from 0 to 1.");
        }
    }
}