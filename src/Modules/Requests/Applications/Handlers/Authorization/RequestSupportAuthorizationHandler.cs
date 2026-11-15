using NoMoreTears.Shared.Common.Authorization;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

/// <summary>
///     Allows a request only to arbiters (PlatformSupport) and platform administrators.
/// </summary>
public abstract class RequestSupportAuthorizationHandler<TRequest>(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<TRequest>(currentUser)
    where TRequest : MediatR.IBaseRequest
{
    public override Task AuthorizeAsync(
        TRequest request,
        CancellationToken cancellationToken)
    {
        RequireRole(typeof(TRequest).Name, UserRole.PlatformSupport);
        return Task.CompletedTask;
    }
}