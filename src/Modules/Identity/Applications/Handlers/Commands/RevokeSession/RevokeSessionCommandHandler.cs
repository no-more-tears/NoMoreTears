using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.RevokeSession;

public sealed class RevokeSessionCommandHandler(
    IUserService userService) : ICommandHandler<RevokeSessionCommand>
{
    public async Task Handle(
        RevokeSessionCommand request,
        CancellationToken cancellationToken)
    {
        await userService.RevokeSessionAsync(
            request.UserId,
            request.SessionId,
            cancellationToken);
    }
}
