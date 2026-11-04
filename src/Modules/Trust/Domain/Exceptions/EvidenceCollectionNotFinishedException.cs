using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when a decision is made before the evidence collection period ends.
/// </summary>
public sealed class EvidenceCollectionNotFinishedException(DateTimeOffset endsAt)
    : DomainException($"A decision can be made only after evidence collection ends at {endsAt:O}.");