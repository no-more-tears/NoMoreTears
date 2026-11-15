using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RefuseRequest;

public sealed class RefuseRequestCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestMasterAuthorizationHandler<RefuseRequestCommand>(currentUser);