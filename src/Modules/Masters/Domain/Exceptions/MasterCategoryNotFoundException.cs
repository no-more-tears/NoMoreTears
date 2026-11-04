using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a category is not in the master profile.
/// </summary>
public sealed class MasterCategoryNotFoundException(Guid categoryId)
    : DomainException($"Category '{categoryId}' is not in the master profile.");