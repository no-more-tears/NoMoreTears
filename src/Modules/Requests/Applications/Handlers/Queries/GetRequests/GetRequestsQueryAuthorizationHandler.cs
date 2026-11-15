using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequests;

public sealed class GetRequestsQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestSupportAuthorizationHandler<GetRequestsQuery>(currentUser);