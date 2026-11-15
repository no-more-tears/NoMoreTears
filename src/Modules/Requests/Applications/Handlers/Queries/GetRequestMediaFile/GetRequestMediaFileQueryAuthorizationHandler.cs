using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetRequestMediaFile;

public sealed class GetRequestMediaFileQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestSupportAuthorizationHandler<GetRequestMediaFileQuery>(currentUser);