using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a catalog category name or description violates its rules.
/// </summary>
public sealed class InvalidCategoryException(string message)
    : DomainException(message);