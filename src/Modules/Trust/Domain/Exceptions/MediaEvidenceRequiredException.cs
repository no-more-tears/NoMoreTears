using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when a client with many rejected complaints submits a complaint without media evidence.
/// </summary>
public sealed class MediaEvidenceRequiredException()
    : DomainException("Complaints of this client are accepted only with media evidence.");