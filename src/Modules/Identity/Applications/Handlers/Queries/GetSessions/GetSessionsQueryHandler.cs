using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Queries.GetSessions;

public sealed class GetSessionsQueryHandler(
    IUserService userService) : IQueryHandler<GetSessionsQuery, IReadOnlyList<SessionResponse>>
{
    public async Task<IReadOnlyList<SessionResponse>> Handle(
        GetSessionsQuery request,
        CancellationToken cancellationToken)
    {
        return await userService.GetSessionsAsync(
            request.UserId,
            request.CurrentSessionId,
            cancellationToken);
    }
}
