using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when evidence contains too many files.
/// </summary>
public sealed class EvidenceFileLimitExceededException(int limit)
    : DomainException($"Evidence can contain at most {limit} files.");