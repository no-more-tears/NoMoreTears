using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Queries.GetMediaUpload;

public sealed class GetMediaUploadQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestClientAuthorizationHandler<GetMediaUploadQuery>(currentUser);