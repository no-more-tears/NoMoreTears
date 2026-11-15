using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Exceptions;

/// <summary>
///     Thrown when an archived category of the catalog is chosen for a request.
/// </summary>
public sealed class CategoryUnavailableException(Guid categoryId)
    : DomainException($"Category '{categoryId}' is archived and cannot be chosen.");