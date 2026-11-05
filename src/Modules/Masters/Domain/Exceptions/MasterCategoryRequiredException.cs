using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Exceptions;

/// <summary>
///     Thrown when a master profile would be left without categories.
/// </summary>
public sealed class MasterCategoryRequiredException()
    : DomainException("A master profile must contain at least one category.");