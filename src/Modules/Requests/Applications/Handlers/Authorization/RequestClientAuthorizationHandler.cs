using NoMoreTears.Shared.Common.Authorization;
using NoMoreTears.Shared.Common.Exceptions.Client;

namespace NoMoreTears.Modules.Requests.Applications.Handlers.Authorization;

/// <summary>
///     A request of a client about the own requests.
/// </summary>
public interface IRequestClientRequest
{
    /// <summary>
    ///     Client on whose behalf the request is sent; it must be the current user.
    /// </summary>
    Guid ClientId { get; }
}

/// <summary>
///     Allows a request only to a user with the Client role, on the own behalf.
///     Ownership of the request itself is checked by the services and the domain.
/// </summary>
public abstract class RequestClientAuthorizationHandler<TRequest>(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<TRequest>(currentUser)
    where TRequest : MediatR.IBaseRequest, IRequestClientRequest
{
    public override Task AuthorizeAsync(
        TRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        RequireRole(typeof(TRequest).Name, UserRole.Client);

        if (CurrentUser.UserId != request.ClientId)
        {
            throw new ForbiddenException("A client can manage only the own requests.");
        }

        return Task.CompletedTask;
    }
}