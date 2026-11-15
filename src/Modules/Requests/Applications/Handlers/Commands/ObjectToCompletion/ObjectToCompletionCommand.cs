using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ObjectToCompletion;

/// <summary>
///     Objects to the completion marked by the master; the request returns to execution.
/// </summary>
public sealed record ObjectToCompletionCommand(
    Guid ClientId,
    Guid RequestId,
    string Reason) : ICommand<RequestResponse>, IRequestClientRequest;