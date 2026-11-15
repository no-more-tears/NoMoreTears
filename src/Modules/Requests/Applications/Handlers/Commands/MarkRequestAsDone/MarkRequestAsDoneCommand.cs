using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.MarkRequestAsDone;

/// <summary>
///     Marks the work as done; the client confirms or the request completes automatically after 48 hours (FR-REQ-06).
/// </summary>
public sealed record MarkRequestAsDoneCommand(
    Guid MasterId,
    Guid RequestId) : ICommand<RequestResponse>, IRequestMasterRequest;