using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when a master tries to accept a request during a cooldown.
/// </summary>
public sealed class MasterCoolingDownException(DateTimeOffset until)
    : DomainException($"The master cannot accept requests until {until:O}.");