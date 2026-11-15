using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.PublishRequest;

/// <summary>
///     Publishes a draft; from this moment the content is fixed (FR-REQ-02).
/// </summary>
public sealed record PublishRequestCommand(
    Guid ClientId,
    Guid RequestId) : ICommand<RequestResponse>, IRequestClientRequest;