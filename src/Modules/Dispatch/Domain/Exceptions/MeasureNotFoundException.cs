using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when a measure is not applied to the master.
/// </summary>
public sealed class MeasureNotFoundException(Guid measureId)
    : DomainException($"Measure '{measureId}' is not applied to the master.");