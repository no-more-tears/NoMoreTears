namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.SettleDispute;

/// <summary>
///     Returns a disputed request to its previous status after Trust closed the incident. Sent by the Requests consumers.
/// </summary>
public sealed record SettleDisputeCommand(
    Guid RequestId,
    Guid IncidentId) : ICommand;