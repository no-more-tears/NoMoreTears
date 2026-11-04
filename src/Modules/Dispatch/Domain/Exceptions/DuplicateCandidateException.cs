using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when the same master is given twice as a candidate.
/// </summary>
public sealed class DuplicateCandidateException(Guid masterId)
    : DomainException($"Master '{masterId}' is given as a candidate more than once.");