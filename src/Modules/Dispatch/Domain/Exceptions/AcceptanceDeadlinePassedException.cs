using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when a request is accepted after its acceptance deadline.
/// </summary>
public sealed class AcceptanceDeadlinePassedException()
    : DomainException("The acceptance deadline of the request has passed.");