namespace NoMoreTears.Shared.Common.Exceptions.Client;

/// <summary>
///     Thrown when the user does not have permission to access the resource.
/// </summary>
public sealed class ForbiddenException(string message)
    : Exception(message);