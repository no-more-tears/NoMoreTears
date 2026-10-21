namespace NoMoreTears.Shared.Common.Exceptions.Client;

/// <summary>
///     Thrown when the client has sent too many requests in a given amount of time.
/// </summary>
public sealed class TooManyRequestsException(string message = "You have exceeded the allowed number of requests.")
    : Exception(message);