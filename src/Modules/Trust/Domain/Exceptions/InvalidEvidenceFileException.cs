using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when an evidence file violates its rules.
/// </summary>
public sealed class InvalidEvidenceFileException(string message)
    : DomainException(message);