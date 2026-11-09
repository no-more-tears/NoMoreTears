using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ChangeFullName;

public sealed class ChangeFullNameCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : MasterOwnerAuthorizationHandler<ChangeFullNameCommand>(currentUser);
