using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnRequestHistory;

public sealed class GetOwnRequestHistoryQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestClientAuthorizationHandler<GetOwnRequestHistoryQuery>(currentUser);