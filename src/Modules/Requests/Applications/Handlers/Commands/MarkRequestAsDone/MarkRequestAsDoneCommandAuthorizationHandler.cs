using NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Commands.MarkRequestAsDone;

public sealed class MarkRequestAsDoneCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : RequestMasterAuthorizationHandler<MarkRequestAsDoneCommand>(currentUser);