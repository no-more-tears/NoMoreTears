using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Modules.Masters.Domain.Exceptions;

namespace NoMoreTears.Modules.Masters.Domain.Entities;

/// <summary>
///     Sanction or temporary measure imposed by Trust and applied to the master's feed access.
/// </summary>
public sealed class Sanction
{
    /// <summary>
    ///     Identifier of the sanction in Trust.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    ///     Kind of the sanction.
    /// </summary>
    public SanctionKind Kind { get; }

    /// <summary>
    ///     Moment the sanction takes effect.
    /// </summary>
    public DateTimeOffset ImposedAt { get; }

    /// <summary>
    ///     Moment the sanction ends by itself.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; }

    /// <summary>
    ///     Moment the sanction was lifted, for example after an appeal.
    /// </summary>
    public DateTimeOffset? LiftedAt { get; private set; }

    /// <summary>
    ///     Returns whether the sanction is in effect at the given moment.
    /// </summary>
    public bool IsActiveAt(DateTimeOffset moment)
    {
        return LiftedAt is null && moment >= ImposedAt && (ExpiresAt is null || moment < ExpiresAt);
    }

    private Sanction(
        Guid id,
        SanctionKind kind,
        DateTimeOffset imposedAt,
        DateTimeOffset? expiresAt)
    {
        Id = id;
        Kind = kind;
        ImposedAt = imposedAt;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    ///     Creates a sanction. Only the <see cref="Aggregates.Master"/> aggregate applies sanctions.
    /// </summary>
    internal static Sanction Create(
        Guid sanctionId, 
        SanctionKind kind, 
        DateTimeOffset imposedAt, 
        DateTimeOffset? expiresAt)
    {
        if (sanctionId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(sanctionId));
        }

        if (expiresAt <= imposedAt)
        {
            throw new InvalidSanctionException("A sanction must expire after it is imposed.");
        }

        return new Sanction(sanctionId, kind, imposedAt, expiresAt);
    }

    internal void Lift(DateTimeOffset liftedAt)
    {
        if (LiftedAt is not null)
        {
            throw new InvalidSanctionException($"Sanction '{Id}' has already been lifted.");
        }

        LiftedAt = liftedAt;
    }
}