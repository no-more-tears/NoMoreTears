using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DuplicateRequest;

public sealed class DuplicateRequestCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestClientAuthorizationHandler<DuplicateRequestCommand>(currentUser);