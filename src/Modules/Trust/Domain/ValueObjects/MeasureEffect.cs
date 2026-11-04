using NoMoreTears.Modules.Trust.Domain.Enums;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Effect of a temporary measure that another context applies,
///     with its own duration if shorter than the measure.
/// </summary>
public sealed record MeasureEffect
{
    /// <summary>
    ///     Kind of the effect.
    /// </summary>
    public EffectKind Kind { get; }

    /// <summary>
    ///     Duration of the effect.
    /// </summary>
    public TimeSpan? Duration { get; }

    private MeasureEffect(
        EffectKind kind,
        TimeSpan? duration)
    {
        Kind = kind;
        Duration = duration;
    }

    /// <summary>
    ///     Creates an effect.
    /// </summary>
    public static MeasureEffect Of(
        EffectKind kind,
        TimeSpan? duration)
    {
        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration), duration, "Effect duration must be positive.");
        }

        return new MeasureEffect(kind, duration);
    }
}