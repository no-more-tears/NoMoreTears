namespace NoMoreTears.Shared.Common.Exceptions.Server;

/// <summary>
///     Thrown when an unexpected server-side error occurs.
/// </summary
public sealed class InternalServerErrorException(string message)
    : Exception(message);