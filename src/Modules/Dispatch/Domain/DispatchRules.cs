using System.Collections.Frozen;

namespace NoMoreTears.Modules.Dispatch.Domain;

/// <summary>
///     Fixed rules of the Dispatch bounded context.
///     Configurable parameters live in <see cref="Aggregates.DispatchPolicy"/>.
/// </summary>
public static class DispatchRules
{
    /// <summary>
    ///     Number of latest acceptance attempts used to detect automation.
    /// </summary>
    public const int AutomationSampleSize = 5;

    /// <summary>
    ///     Default bonus added to the score of wave 1 candidates of a republished request.
    /// </summary>
    public const double DefaultRepublicationBonus = 0.10;

    /// <summary>
    ///     Default share by which the score of a restricted master is lowered.
    /// </summary>
    public const double DefaultRestrictionPenalty = 0.20;

    /// <summary>
    ///     Tolerance used when comparing configured shares and weights.
    /// </summary>
    public const double Tolerance = 1e-9;

    /// <summary>
    ///     Median reaction time below which acceptance attempts are treated as automated.
    /// </summary>
    public static readonly TimeSpan AutomationReactionThreshold = TimeSpan.FromSeconds(2);

    /// <summary>
    ///     Window in which acceptance attempts are counted for the rate limit.
    /// </summary>
    public static readonly TimeSpan AcceptanceAttemptWindow = TimeSpan.FromMinutes(1);

    /// <summary>
    ///     Arrival intervals a master can choose when accepting a request.
    /// </summary>
    public static FrozenSet<TimeSpan> AllowedArrivalIntervals { get; } = new[]
    {
        TimeSpan.FromHours(2),
        TimeSpan.FromHours(6),
        TimeSpan.FromHours(24)
    }.ToFrozenSet();
}