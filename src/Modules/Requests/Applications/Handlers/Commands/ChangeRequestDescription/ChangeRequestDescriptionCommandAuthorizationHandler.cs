using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ChangeRequestDescription;

public sealed class ChangeRequestDescriptionCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestClientAuthorizationHandler<ChangeRequestDescriptionCommand>(currentUser);