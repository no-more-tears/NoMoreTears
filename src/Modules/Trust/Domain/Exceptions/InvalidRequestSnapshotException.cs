using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when request data received from Requests is inconsistent.
/// </summary>
public sealed class InvalidRequestSnapshotException(string message)
    : DomainException(message);