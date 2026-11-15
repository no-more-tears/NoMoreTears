using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Modules.Requests.Domain.Exceptions;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Aggregates;

/// <summary>
///     Restriction of publication imposed on a client by Trust (a sanction or a temporary measure).
///     The identifier is the identifier of the sanction or measure in Trust, so it can be lifted later.
/// </summary>
public sealed class ClientRestriction : AuditableEntity<Guid>
{
    /// <summary>
    ///     Restricted client.
    /// </summary>
    public Guid ClientId { get; private set; }

    /// <summary>
    ///     What the restriction does.
    /// </summary>
    public RestrictionKind Kind { get; private set; }

    /// <summary>
    ///     Moment from which the restriction applies.
    /// </summary>
    public DateTimeOffset ImposedAt { get; private set; }

    /// <summary>
    ///     Moment the restriction ends by itself; <c>null</c> means until it is lifted.
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; private set; }

    /// <summary>
    ///     Moment the restriction was lifted by Trust.
    /// </summary>
    public DateTimeOffset? LiftedAt { get; private set; }

    /// <summary>
    ///     Whether the restriction has been lifted.
    /// </summary>
    public bool IsLifted => LiftedAt is not null;

    private ClientRestriction(
        Guid id,
        Guid clientId,
        RestrictionKind kind,
        DateTimeOffset imposedAt,
        DateTimeOffset? expiresAt)
    {
        Id = id;
        ClientId = clientId;
        Kind = kind;
        ImposedAt = imposedAt;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    ///     Records a restriction imposed by Trust.
    /// </summary>
    public static ClientRestriction Create(
        Guid restrictionId,
        Guid clientId,
        RestrictionKind kind,
        DateTimeOffset imposedAt,
        DateTimeOffset? expiresAt,
        DateTimeOffset createdAt)
    {
        if (restrictionId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(restrictionId));
        }

        if (clientId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(clientId));
        }

        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(nameof(kind), kind, "Restriction kind is unknown.");
        }

        if (expiresAt <= imposedAt)
        {
            throw new InvalidRestrictionException("A restriction must expire after it is imposed.");
        }

        var restriction = new ClientRestriction(restrictionId, clientId, kind, imposedAt, expiresAt);
        restriction.MarkAsCreated(createdAt);

        return restriction;
    }

    /// <summary>
    ///     Lifts the restriction before its end.
    /// </summary>
    public void Lift(DateTimeOffset liftedAt)
    {
        if (IsLifted)
        {
            throw new InvalidRestrictionException("The restriction is already lifted.");
        }

        LiftedAt = liftedAt;
        MarkAsUpdated(liftedAt);
    }

    /// <summary>
    ///     Returns whether the restriction applies at the given moment.
    /// </summary>
    public bool IsActiveAt(DateTimeOffset moment)
    {
        return !IsLifted && moment >= ImposedAt && (ExpiresAt is null || moment < ExpiresAt);
    }
}