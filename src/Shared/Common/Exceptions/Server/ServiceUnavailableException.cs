namespace NoMoreTears.Shared.Common.Exceptions.Server;

/// <summary>
///     Thrown when the service is temporarily unavailable.
/// </summary>
public sealed class ServiceUnavailableException(string message)
    : Exception(message);