namespace NoMoreTears.Shared.Common.Exceptions.Integration;

/// <summary>
///     Thrown when an external service is unavailable or unreachable.
/// </summary>
public class ExternalUnavailableException(string message)
    : Exception(message);