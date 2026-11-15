using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ExtendRequest;

/// <summary>
///     Extends the waiting for acceptance of an expired request (FR-REQ-03).
/// </summary>
public sealed record ExtendRequestCommand(
    Guid ClientId,
    Guid RequestId) : ICommand<RequestResponse>, IRequestClientRequest;