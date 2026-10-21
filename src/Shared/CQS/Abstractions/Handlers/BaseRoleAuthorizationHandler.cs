using MediatR;
using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Exceptions.Client;

namespace NoMoreTears.Shared.CQS.Abstractions.Handlers;

public abstract class BaseRoleAuthorizationHandler<TRequest>(
    ICurrentUserSession currentUser)
    : IAuthorizationHandler<TRequest> 
    where TRequest : IBaseRequest
{
    protected ICurrentUserSession CurrentUser { get; } = currentUser;

    public abstract Task AuthorizeAsync(TRequest request, CancellationToken cancellationToken);

    protected void RequireRole(string operation, params UserRole[] allowedRoles)
    {
        var userRoles = CurrentUser.Roles;

        if (userRoles is null || userRoles.Count == 0)
        {
            throw new ForbiddenException(
                BuildAccessDeniedMessage(operation, allowedRoles));
        }

        if (userRoles.Any(r => r == UserRole.PlatformAdministrator))
        {
            return;
        }

        if (userRoles.Any(r => allowedRoles.Any(a => a == r)))
        {
            return;
        }

        throw new ForbiddenException(
            BuildAccessDeniedMessage(operation, allowedRoles));
    }

    /// <summary>
    ///     Builds a human-readable access denied message.
    /// </summary>
    private static string BuildAccessDeniedMessage(
        string operation,
        IEnumerable<UserRole> allowedRoles)
    {
        var roles = string.Join(", ", allowedRoles.Select(r => r.ToString()));
        return $"Access denied for operation '{operation}'. Required roles: {roles}.";
    }
}