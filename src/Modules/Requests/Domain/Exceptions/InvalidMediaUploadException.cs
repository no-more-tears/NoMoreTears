using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when an uploaded photo violates its rules.
/// </summary>
public sealed class InvalidMediaUploadException(string message)
    : DomainException(message);