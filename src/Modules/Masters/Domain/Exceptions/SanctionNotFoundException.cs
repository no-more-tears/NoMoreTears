using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a sanction is not applied to the master.
/// </summary>
public sealed class SanctionNotFoundException(Guid sanctionId)
    : DomainException($"Sanction '{sanctionId}' is not applied to the master.");