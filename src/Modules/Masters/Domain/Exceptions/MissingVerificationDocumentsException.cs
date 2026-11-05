using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when documents required for verification have not been uploaded.
/// </summary>
public sealed class MissingVerificationDocumentsException(string message)
    : DomainException(message);