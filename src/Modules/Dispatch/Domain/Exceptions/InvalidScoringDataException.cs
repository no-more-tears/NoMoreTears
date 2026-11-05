using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when data used for scoring is out of range.
/// </summary>
public sealed class InvalidScoringDataException(string message)
    : DomainException(message);