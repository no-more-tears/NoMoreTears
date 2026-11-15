namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ExpireOverdueRequests;

/// <summary>
///     Expires a batch of published requests whose acceptance deadline has passed. Sent by the background jobs.
/// </summary>
public sealed record ExpireOverdueRequestsCommand(
    int BatchSize) : ICommand<int>;