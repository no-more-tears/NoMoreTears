using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Trust.Domain.Exceptions;

/// <summary>
///     Thrown when data for the reputation calculation is out of range.
/// </summary>
public sealed class InvalidReputationInputException(string message)
    : DomainException(message);