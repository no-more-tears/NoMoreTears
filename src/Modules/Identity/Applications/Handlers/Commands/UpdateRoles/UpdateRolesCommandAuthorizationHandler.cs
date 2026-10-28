using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Exceptions.Client;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.UpdateRoles;

public sealed class UpdateRolesCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<UpdateRolesCommand>(currentUser)
{
    private const string SelfDemotionMessage = "Administrators cannot remove their own administrator role.";

    public override Task AuthorizeAsync(
        UpdateRolesCommand request,
        CancellationToken cancellationToken)
    {
        RequireRole(nameof(UpdateRolesCommand), UserRole.PlatformAdministrator);

        // Prevents an administrator from locking themselves out of user administration.
        // Authorization runs before validation, so a missing role list is left to the validator.
        if (request.UserId == CurrentUser.UserId
            && request.Roles is not null
            && !request.Roles.Contains(UserRole.PlatformAdministrator))
        {
            throw new ForbiddenException(SelfDemotionMessage);
        }

        return Task.CompletedTask;
    }
}
