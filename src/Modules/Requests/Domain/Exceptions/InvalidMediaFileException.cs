using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when a media file violates its rules.
/// </summary>
public sealed class InvalidMediaFileException(string message)
    : DomainException(message);