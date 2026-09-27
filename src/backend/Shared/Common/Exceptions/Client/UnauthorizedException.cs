namespace NoMoreTears.Shared.Common.Exceptions.Client;

/// <summary>
///     Thrown when authentication is required but was not provided or is invalid.
/// </summary>
public sealed class UnauthorizedException(string message = "User is not authenticated.")
    : Exception(message);