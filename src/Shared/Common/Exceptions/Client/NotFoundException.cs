namespace NoMoreTears.Shared.Common.Exceptions.Client;

/// <summary>
///     Thrown when the requested resource could not be found.
/// </summary>
public sealed class NotFoundException(string message)
    : Exception(message);