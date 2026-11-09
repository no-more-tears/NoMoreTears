using NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;
using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Queries.GetCurrentMaster;

public sealed class GetCurrentMasterQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : MasterOwnerAuthorizationHandler<GetCurrentMasterQuery>(currentUser);
