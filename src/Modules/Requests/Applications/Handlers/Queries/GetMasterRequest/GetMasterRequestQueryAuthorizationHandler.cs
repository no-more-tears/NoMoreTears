using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterRequest;

public sealed class GetMasterRequestQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestMasterAuthorizationHandler<GetMasterRequestQuery>(currentUser);