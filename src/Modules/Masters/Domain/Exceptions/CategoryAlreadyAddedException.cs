using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a category is added to a master profile twice.
/// </summary>
public sealed class CategoryAlreadyAddedException(Guid categoryId)
    : DomainException($"Category '{categoryId}' is already in the master profile.");