using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.AcceptRequest;

public sealed class AcceptRequestCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestMasterAuthorizationHandler<AcceptRequestCommand>(currentUser);