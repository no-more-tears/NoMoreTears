using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when sanction parameters are invalid.
/// </summary>
public sealed class InvalidSanctionException(string message)
    : DomainException(message);