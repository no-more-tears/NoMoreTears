namespace NoMoreTears.Shared.Common.Exceptions.Integration;

/// <summary>
///     Thrown when an external service returns an error response.
/// </summary>
public class ExternalClientException(string message)
    : Exception(message);