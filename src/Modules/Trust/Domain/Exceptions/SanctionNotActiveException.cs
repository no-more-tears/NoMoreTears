using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when an inactive sanction is lifted.
/// </summary>
public sealed class SanctionNotActiveException(Guid sanctionId)
    : DomainException($"Sanction '{sanctionId}' is not active.");