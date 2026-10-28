using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(
    IUserService userService) : IQueryHandler<GetCurrentUserQuery, UserResponse>
{
    public async Task<UserResponse> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        return await userService.GetAsync(
            request.UserId,
            cancellationToken);
    }
}
