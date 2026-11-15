namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.CompleteOverdueRequests;

/// <summary>
///     Completes a batch of requests whose confirmation period has passed without an objection. Sent by the background jobs.
/// </summary>
public sealed record CompleteOverdueRequestsCommand(
    int BatchSize) : ICommand<int>;