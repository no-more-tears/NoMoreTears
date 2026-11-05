namespace NoMoreTears.Modules.Trust.Domain.Enums;

/// <summary>
///     Automatic blocking rule that creates a temporary measure.
/// </summary>
public enum AutomaticRule
{
    RepeatedNoShows,
    MasterCancellationSeries,
    SuspectedAutomation,
    SelfBoosting,
    DuplicateAccount,
    ClientAbuse
}