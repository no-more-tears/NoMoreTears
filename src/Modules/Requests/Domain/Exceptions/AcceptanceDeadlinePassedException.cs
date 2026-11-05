using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when a master tries to accept a request after its acceptance deadline.
/// </summary>
public sealed class AcceptanceDeadlinePassedException()
    : DomainException("The acceptance deadline of the request has passed.");