using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ConfirmCompletion;

/// <summary>
///     Confirms the completion of the work marked as done by the master (FR-REQ-06).
/// </summary>
public sealed record ConfirmCompletionCommand(
    Guid ClientId,
    Guid RequestId) : ICommand<RequestResponse>, IRequestClientRequest;