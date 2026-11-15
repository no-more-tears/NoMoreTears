using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequest;

/// <summary>
///     Returns a request of the client.
/// </summary>
public sealed record GetOwnRequestQuery(
    Guid ClientId,
    Guid RequestId) : IQuery<RequestResponse>, IRequestClientRequest;