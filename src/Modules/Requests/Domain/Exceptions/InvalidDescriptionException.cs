using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when a request description violates its rules.
/// </summary>
public sealed class InvalidDescriptionException(string message)
    : DomainException(message);