using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMasterRequestHistory;

public sealed class GetMasterRequestHistoryQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestMasterAuthorizationHandler<GetMasterRequestHistoryQuery>(currentUser);