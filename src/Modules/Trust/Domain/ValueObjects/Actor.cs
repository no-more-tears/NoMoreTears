using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Modules.Trust.Domain.Exceptions;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Whoever performed an audited action:
///     the platform, a participant, an arbiter or an administrator.
/// </summary>
public sealed record Actor
{
    /// <summary>
    ///     The platform itself: automatic rules and deadlines.
    /// </summary>
    public static Actor System { get; } = new(ActorRole.System, null);

    /// <summary>
    ///     Role of the actor.
    /// </summary>
    public ActorRole Role { get; }

    /// <summary>
    ///     Identifier of the user.
    /// </summary>
    public Guid? UserId { get; }

    private Actor(
        ActorRole role,
        Guid? userId)
    {
        Role = role;
        UserId = userId;
    }

    /// <summary>
    ///     Creates an actor for a user acting in the given role.
    /// </summary>
    public static Actor Of(
        ActorRole role,
        Guid userId)
    {
        if (role is ActorRole.System || !Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(nameof(role), role, "Use Actor.System for the platform.");
        }

        if (userId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(userId));
        }

        return new Actor(role, userId);
    }

    /// <summary>
    ///     Creates an actor for a participant of a request.
    /// </summary>
    public static Actor Of(Party party)
    {
        ArgumentNullException.ThrowIfNull(party);

        return Of(party.Role is PartyRole.Client ? ActorRole.Client : ActorRole.Master, party.UserId);
    }
}