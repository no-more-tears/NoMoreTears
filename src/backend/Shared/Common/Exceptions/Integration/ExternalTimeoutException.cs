namespace NoMoreTears.Shared.Common.Exceptions.Integration;

/// <summary>
///     Thrown when a request to an external service times out.
/// </summary>
public class ExternalTimeoutException(string message)
    : Exception(message); 