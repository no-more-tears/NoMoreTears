using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when a master chooses an arrival interval outside the allowed list.
/// </summary>
public sealed class InvalidArrivalIntervalException(TimeSpan interval)
    : DomainException($"Arrival interval '{interval}' is not one of the allowed intervals.");