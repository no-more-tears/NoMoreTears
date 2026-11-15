using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequestHistory;

public sealed class GetRequestHistoryQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestSupportAuthorizationHandler<GetRequestHistoryQuery>(currentUser);