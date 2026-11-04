using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Dispatch.Domain.Exceptions;

/// <summary>
///     Thrown when the next wave of a distribution cannot be opened.
/// </summary>
public sealed class WaveAdvanceException(string message)
    : DomainException(message);