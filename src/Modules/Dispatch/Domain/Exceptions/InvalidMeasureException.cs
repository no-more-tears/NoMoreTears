using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when measure parameters are invalid.
/// </summary>
public sealed class InvalidMeasureException(string message)
    : DomainException(message);