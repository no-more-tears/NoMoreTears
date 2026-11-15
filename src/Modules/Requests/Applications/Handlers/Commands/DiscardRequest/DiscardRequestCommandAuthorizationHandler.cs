using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DiscardRequest;

public sealed class DiscardRequestCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestClientAuthorizationHandler<DiscardRequestCommand>(currentUser);