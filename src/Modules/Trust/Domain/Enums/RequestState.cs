namespace NoMoreTears.Modules.Trust.Domain.Enums;

/// <summary>
///     State of a request as published by Requests, reduced to what Trust needs (Conformist).
/// </summary>
public enum RequestState
{
    InProgress,
    Completed,
    Failed,
    Other
}