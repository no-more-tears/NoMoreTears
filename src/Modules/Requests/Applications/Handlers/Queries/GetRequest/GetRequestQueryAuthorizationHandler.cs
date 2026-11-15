using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequest;

public sealed class GetRequestQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestSupportAuthorizationHandler<GetRequestQuery>(currentUser);