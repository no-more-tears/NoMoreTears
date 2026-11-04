namespace NoMoreTears.Modules.Requests.Domain.Enums;

/// <summary>
///     Status of a request in its lifecycle state machine.
/// </summary>
public enum RequestStatus
{
    Draft,
    Published,
    Expired,
    InProgress,
    AwaitingConfirmation,
    Completed,
    Failed,
    Cancelled,
    Disputed
}