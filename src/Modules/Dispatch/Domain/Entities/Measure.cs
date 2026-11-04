using NoMoreTears.Modules.Dispatch.Domain.Enums;
using NoMoreTears.Modules.Dispatch.Domain.Exceptions;

namespace NoMoreTears.Modules.Dispatch.Domain.Entities;

/// <summary>
///     Effect of a Trust sanction or temporary measure on dispatching.
/// </summary>
public sealed class Measure
{
    /// <summary>
    ///     Identifier of the measure in Trust.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    ///     Effect on dispatching.
    /// </summary>
    public MeasureKind Kind { get; }

    /// <summary>
    ///     Moment the measure takes effect.
    /// </summary>
    public DateTimeOffset StartsAt { get; }

    /// <summary>
    ///     Moment the measure ends by itself.
    /// </summary>
    public DateTimeOffset? EndsAt { get; }

    /// <summary>
    ///     Moment the measure was lifted.
    /// </summary>
    public DateTimeOffset? LiftedAt { get; private set; }

    /// <summary>
    ///     Returns whether the measure is in effect at the given moment.
    /// </summary>
    public bool IsActiveAt(DateTimeOffset moment)
    {
        return LiftedAt is null && moment >= StartsAt && (EndsAt is null || moment < EndsAt);
    }

    private Measure(
        Guid id,
        MeasureKind kind,
        DateTimeOffset startsAt,
        DateTimeOffset? endsAt)
    {
        Id = id;
        Kind = kind;
        StartsAt = startsAt;
        EndsAt = endsAt;
    }

    /// <summary>
    ///     Creates a measure. Only the <see cref="Aggregates.Master"/> aggregate applies measures.
    /// </summary>
    internal static Measure Create(
        Guid measureId,
        MeasureKind kind,
        DateTimeOffset startsAt,
        DateTimeOffset? endsAt)
    {
        if (measureId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(measureId));
        }

        if (!Enum.IsDefined(kind))
        {
            throw new InvalidMeasureException($"Measure kind '{kind}' is unknown.");
        }

        if (endsAt <= startsAt)
        {
            throw new InvalidMeasureException("A measure must end after it starts.");
        }

        return new Measure(measureId, kind, startsAt, endsAt);
    }

    internal void Lift(DateTimeOffset liftedAt)
    {
        if (LiftedAt is not null)
        {
            throw new InvalidMeasureException($"Measure '{Id}' has already been lifted.");
        }

        LiftedAt = liftedAt;
    }
}