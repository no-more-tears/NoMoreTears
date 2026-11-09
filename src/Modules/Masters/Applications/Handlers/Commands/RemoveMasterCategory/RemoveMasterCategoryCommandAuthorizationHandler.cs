using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Commands.RemoveMasterCategory;

public sealed class RemoveMasterCategoryCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : MasterOwnerAuthorizationHandler<RemoveMasterCategoryCommand>(currentUser);
