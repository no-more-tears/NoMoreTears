using NoMoreTears.Modules.Requests.Domain.Enums;
using NoMoreTears.Modules.Requests.Domain.Exceptions;

namespace NoMoreTears.Modules.Requests.Domain.ValueObjects;

/// <summary>
///     Party that initiated a status change.
/// </summary>
public sealed record Initiator
{
    /// <summary>
    ///     The platform itself: deadlines, automatic rules and decisions of other contexts.
    /// </summary>
    public static Initiator System { get; } = new(InitiatorRole.System, null);

    /// <summary>
    ///     Role of the initiator.
    /// </summary>
    public InitiatorRole Role { get; }

    /// <summary>
    ///     Identifier of the user.
    /// </summary>
    public Guid? UserId { get; }

    private Initiator(
        InitiatorRole role,
        Guid? userId)
    {
        Role = role;
        UserId = userId;
    }

    /// <summary>
    ///     Creates an initiator for a user acting in the given role.
    /// </summary>
    public static Initiator Of(InitiatorRole role, Guid userId)
    {
        if (role is InitiatorRole.System)
        {
            throw new ArgumentOutOfRangeException(nameof(role), role, "Use Initiator.System for the platform.");
        }

        if (userId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(userId));
        }

        return new Initiator(role, userId);
    }
}