using NoMoreTears.Modules.Trust.Domain.Exceptions;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Request outcomes of a master used for reliability and for the public profile.
/// </summary>
public sealed record ReliabilityIndicators
{
    /// <summary>
    ///     Requests accepted over 90 days.
    /// </summary>
    public int AcceptedLast90Days { get; }

    /// <summary>
    ///     Requests completed over 90 days.
    /// </summary>
    public int CompletedLast90Days { get; }

    /// <summary>
    ///     Confirmed no-shows over 90 days.
    /// </summary>
    public int NoShowsLast90Days { get; }

    /// <summary>
    ///     Requests completed over the whole time on the platform.
    /// </summary>
    public int CompletedTotal { get; }

    /// <summary>
    ///     Share of accepted requests that were completed.
    /// </summary>
    public double CompletionRate => AcceptedLast90Days == 0 ? 0 : (double)CompletedLast90Days / AcceptedLast90Days;

    /// <summary>
    ///     Share of accepted requests that ended with a no-show.
    /// </summary>
    public double NoShowRate => AcceptedLast90Days == 0 ? 0 : (double)NoShowsLast90Days / AcceptedLast90Days;

    private ReliabilityIndicators(
        int acceptedLast90Days,
        int completedLast90Days,
        int noShowsLast90Days,
        int completedTotal)
    {
        AcceptedLast90Days = acceptedLast90Days;
        CompletedLast90Days = completedLast90Days;
        NoShowsLast90Days = noShowsLast90Days;
        CompletedTotal = completedTotal;
    }

    /// <summary>
    ///     Creates reliability indicators.
    /// </summary>
    public static ReliabilityIndicators Of(
        int acceptedLast90Days,
        int completedLast90Days,
        int noShowsLast90Days,
        int completedTotal)
    {
        if (acceptedLast90Days < 0 || completedLast90Days < 0 || noShowsLast90Days < 0 || completedTotal < 0)
        {
            throw new InvalidReputationInputException("Request counters must not be negative.");
        }

        if (completedLast90Days + noShowsLast90Days > acceptedLast90Days || completedLast90Days > completedTotal)
        {
            throw new InvalidReputationInputException("Completed and no-show counters exceed accepted or total requests.");
        }

        return new ReliabilityIndicators(acceptedLast90Days, completedLast90Days, noShowsLast90Days, completedTotal);
    }
}