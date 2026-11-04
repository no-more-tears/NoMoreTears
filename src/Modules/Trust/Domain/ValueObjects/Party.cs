using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Modules.Trust.Domain.Exceptions;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Participant of a request: the client or the master.
/// </summary>
public sealed record Party
{
    /// <summary>
    ///     Role in the request.
    /// </summary>
    public PartyRole Role { get; }

    /// <summary>
    ///     Identity user identifier.
    /// </summary>
    public Guid UserId { get; }

    private Party(
        PartyRole role,
        Guid userId)
    {
        Role = role;
        UserId = userId;
    }

    /// <summary>
    ///     Creates a party.
    /// </summary>
    public static Party Of(
        PartyRole role,
        Guid userId)
    {
        if (!Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(nameof(role), role, "Unknown party role.");
        }

        if (userId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(userId));
        }

        return new Party(role, userId);
    }
}