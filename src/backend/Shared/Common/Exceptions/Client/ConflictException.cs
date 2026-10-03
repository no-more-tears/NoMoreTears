namespace NoMoreTears.Shared.Common.Exceptions.Client;

/// <summary>
///     Thrown when a request conflicts with the current state of the resource.
/// </summary>
public sealed class ConflictException(string message)
    : Exception(message);