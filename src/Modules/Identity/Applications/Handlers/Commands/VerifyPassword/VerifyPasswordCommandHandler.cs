using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.VerifyPassword;

public sealed class VerifyPasswordCommandHandler(
    IUserService userService) : ICommandHandler<VerifyPasswordCommand>
{
    public async Task Handle(
        VerifyPasswordCommand request,
        CancellationToken cancellationToken)
    {
        await userService.VerifyPasswordAsync(
            request.UserId,
            request.Password,
            cancellationToken);
    }
}
