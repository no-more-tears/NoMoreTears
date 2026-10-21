namespace NoMoreTears.Shared.Common.Exceptions.Client;

/// <summary>
///     Thrown when the requested resource is no longer available and has been permanently removed.
/// </summary>
public sealed class GoneException(string message)
    : Exception(message);