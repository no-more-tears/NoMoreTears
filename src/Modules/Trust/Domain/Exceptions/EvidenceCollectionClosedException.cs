using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when evidence is submitted after the collection period.
/// </summary>
public sealed class EvidenceCollectionClosedException(DateTimeOffset closedAt)
    : DomainException($"Evidence collection closed at {closedAt:O}.");