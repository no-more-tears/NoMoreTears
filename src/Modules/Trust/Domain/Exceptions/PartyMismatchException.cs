using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when someone acts who is not a party of the request or the incident.
/// </summary>
public sealed class PartyMismatchException(string message)
    : DomainException(message);