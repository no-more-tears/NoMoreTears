using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequests;

public sealed class GetOwnRequestsQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestClientAuthorizationHandler<GetOwnRequestsQuery>(currentUser);