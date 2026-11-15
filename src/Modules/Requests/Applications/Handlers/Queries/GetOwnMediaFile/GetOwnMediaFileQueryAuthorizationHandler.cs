using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetOwnMediaFile;

public sealed class GetOwnMediaFileQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestClientAuthorizationHandler<GetOwnMediaFileQuery>(currentUser);