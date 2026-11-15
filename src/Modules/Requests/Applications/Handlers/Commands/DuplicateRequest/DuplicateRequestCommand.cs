using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DuplicateRequest;

/// <summary>
///     Creates a draft with the address and category of an earlier request of the client (FR-REQ-11).
/// </summary>
public sealed record DuplicateRequestCommand(
    Guid ClientId,
    Guid RequestId,
    string Description) : ICommand<RequestResponse>, IRequestClientRequest;