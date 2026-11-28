using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Exceptions.Client;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

/// <summary>
///     A request of a master about requests the master may see or execute.
/// </summary>
public interface IRequestMasterRequest
{
    /// <summary>
    ///     Master on whose behalf the request is sent; it must be the current user.
    /// </summary>
    Guid MasterId { get; }
}

/// <summary>
///     Allows a request only to a user with the Master role (a verified master), on the own behalf.
///     Access to the request itself is checked by the services and the domain.
/// </summary>
public abstract class RequestMasterAuthorizationHandler<TRequest>(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<TRequest>(currentUser)
    where TRequest : MediatR.IBaseRequest, IRequestMasterRequest
{
    public override Task AuthorizeAsync(
        TRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        RequireRole(typeof(TRequest).Name, UserRole.Master);

        if (CurrentUser.UserId != request.MasterId)
        {
            throw new ForbiddenException("A master can act only on the own behalf.");
        }

        return Task.CompletedTask;
    }
}