namespace NoMoreTears.Shared.Common.Exceptions.Client;

/// <summary>
///     Thrown when the request is invalid or cannot be processed due to client error.
/// </summary>
public sealed class BadRequestException(string message)
    : Exception(message);