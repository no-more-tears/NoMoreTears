using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.CancelRequest;

/// <summary>
///     Cancels a request; after acceptance a reason is required and the cancellation counts in abuse signals (FR-REQ-05).
/// </summary>
public sealed record CancelRequestCommand(
    Guid ClientId,
    Guid RequestId,
    string? Reason) : ICommand<RequestResponse>, IRequestClientRequest;