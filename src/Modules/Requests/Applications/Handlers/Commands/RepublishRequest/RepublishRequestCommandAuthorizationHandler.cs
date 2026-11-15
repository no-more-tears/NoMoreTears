using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.RepublishRequest;

public sealed class RepublishRequestCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestClientAuthorizationHandler<RepublishRequestCommand>(currentUser);