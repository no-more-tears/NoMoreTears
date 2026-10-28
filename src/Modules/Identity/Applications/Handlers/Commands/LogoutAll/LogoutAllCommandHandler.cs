using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.LogoutAll;

public sealed class LogoutAllCommandHandler(
    IIdentityService identityService) : ICommandHandler<LogoutAllCommand>
{
    public async Task Handle(
        LogoutAllCommand request,
        CancellationToken cancellationToken)
    {
        await identityService.LogoutAllAsync(
            request.UserId,
            cancellationToken);
    }
}
