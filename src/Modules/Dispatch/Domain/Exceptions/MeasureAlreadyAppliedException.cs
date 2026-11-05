using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when the same measure is applied twice.
/// </summary>
public sealed class MeasureAlreadyAppliedException(Guid measureId)
    : DomainException($"Measure '{measureId}' has already been applied.");