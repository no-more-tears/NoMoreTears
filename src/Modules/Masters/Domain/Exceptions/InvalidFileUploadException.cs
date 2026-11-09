using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when an uploaded file violates its rules.
/// </summary>
public sealed class InvalidFileUploadException(string message)
    : DomainException(message);
