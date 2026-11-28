using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Exceptions.Client;

namespace NoMoreTears.Modules.Masters.Applications.Handlers.Authorization;

/// <summary>
///     A request of a master about the own profile.
/// </summary>
public interface IMasterOwnedRequest
{
    /// <summary>
    ///     Profile the request is about; it must belong to the current user.
    /// </summary>
    Guid MasterId { get; }
}

/// <summary>
///     Allows a request only about the own profile, to a client who is becoming a master or to a master.
/// </summary>
public abstract class MasterOwnerAuthorizationHandler<TRequest>(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<TRequest>(currentUser)
    where TRequest : MediatR.IBaseRequest, IMasterOwnedRequest
{
    public override Task AuthorizeAsync(
        TRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        RequireRole(typeof(TRequest).Name, UserRole.Client, UserRole.Master);

        if (CurrentUser.UserId != request.MasterId)
        {
            throw new ForbiddenException("A master can manage only the own profile.");
        }

        return Task.CompletedTask;
    }
}