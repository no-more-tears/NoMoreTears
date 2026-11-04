using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when the same sanction is applied twice.
/// </summary>
public sealed class SanctionAlreadyAppliedException(Guid sanctionId)
    : DomainException($"Sanction '{sanctionId}' has already been applied.");