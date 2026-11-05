using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when a sanction is imposed without a basis or against the ladder.
/// </summary>
public sealed class SanctionNotAllowedException(string message)
    : DomainException(message);