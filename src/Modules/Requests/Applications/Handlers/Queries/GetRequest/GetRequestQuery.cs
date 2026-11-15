namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequest;

/// <summary>
///     Returns any request for arbiters, with the address.
/// </summary>
public sealed record GetRequestQuery(
    Guid RequestId) : IQuery<RequestResponse>;