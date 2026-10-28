using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.UpdateMfa;

public sealed class UpdateMfaCommandHandler(
    IUserService userService) : ICommandHandler<UpdateMfaCommand>
{
    public async Task Handle(
        UpdateMfaCommand request,
        CancellationToken cancellationToken)
    {
        await userService.UpdateMfaAsync(
            request.UserId,
            request.Password,
            request.Enabled,
            cancellationToken);
    }
}
