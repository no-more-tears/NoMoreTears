using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterRequest;

/// <summary>
///     Returns a request assigned to the master or a published request the master may accept, without the address.
/// </summary>
public sealed record GetMasterRequestQuery(
    Guid MasterId,
    Guid RequestId) : IQuery<RequestResponse>, IRequestMasterRequest;