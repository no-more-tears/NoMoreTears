namespace NoMoreTears.Modules.Dispatch.Domain.Enums;

/// <summary>
///     Effect of a Trust sanction or temporary measure on dispatching.
/// </summary>
public enum MeasureKind
{
    Restriction,
    WaveOneExclusion,
    Cooldown,
    AcceptanceThrottle
}