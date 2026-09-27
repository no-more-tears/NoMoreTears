namespace NoMoreTears.Shared.Common.Exceptions.Integration;

/// <summary>
///     Thrown when an external authentication provider fails or returns an error.
/// </summary>
public class ExternalAuthException(string message)
    : Exception(message);