namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DisputeRequest;

/// <summary>
///     Moves a request to dispute after Trust accepted a complaint (FR-TRS-04). Sent by the Requests consumers.
/// </summary>
public sealed record DisputeRequestCommand(
    Guid RequestId,
    Guid IncidentId) : ICommand;