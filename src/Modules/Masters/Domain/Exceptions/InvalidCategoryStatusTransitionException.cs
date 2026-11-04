using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when an action is not allowed in the current status of a catalog category.
/// </summary>
public sealed class InvalidCategoryStatusTransitionException(CategoryStatus status, string action)
    : DomainException($"Cannot {action} a category in status '{status}'.");