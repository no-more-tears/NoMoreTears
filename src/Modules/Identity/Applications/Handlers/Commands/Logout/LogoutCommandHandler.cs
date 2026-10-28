using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Logout;

public sealed class LogoutCommandHandler(
    IIdentityService identityService) : ICommandHandler<LogoutCommand>
{
    public async Task Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        await identityService.LogoutAsync(
            request.RefreshToken,
            cancellationToken);
    }
}
