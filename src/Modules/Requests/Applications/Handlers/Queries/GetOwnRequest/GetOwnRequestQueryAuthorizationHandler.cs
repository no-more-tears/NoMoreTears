using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequest;

public sealed class GetOwnRequestQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestClientAuthorizationHandler<GetOwnRequestQuery>(currentUser);