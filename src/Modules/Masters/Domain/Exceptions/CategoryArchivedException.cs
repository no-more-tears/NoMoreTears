using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a master chooses a category that is archived in the catalog.
/// </summary>
public sealed class CategoryArchivedException(Guid categoryId)
    : DomainException($"Category '{categoryId}' is archived and cannot be chosen.");