using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when an action is performed by an actor with an inappropriate role.
/// </summary>
public sealed class ActorRoleMismatchException(string message)
    : DomainException(message);