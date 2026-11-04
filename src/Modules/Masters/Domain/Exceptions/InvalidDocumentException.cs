using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a verification document violates its rules.
/// </summary>
public sealed class InvalidDocumentException(string message)
    : DomainException(message);