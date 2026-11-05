using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a verification document is not in the master profile.
/// </summary>
public sealed class DocumentNotFoundException(Guid documentId)
    : DomainException($"Document '{documentId}' is not in the master profile.");