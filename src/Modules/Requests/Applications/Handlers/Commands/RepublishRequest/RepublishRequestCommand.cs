using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RepublishRequest;

/// <summary>
///     Publishes a failed request again with a raised scoring priority (FR-REQ-07).
/// </summary>
public sealed record RepublishRequestCommand(
    Guid ClientId,
    Guid RequestId) : ICommand<RequestResponse>, IRequestClientRequest;