using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.DeactivateUser;

public sealed class DeactivateUserCommandHandler(
    IUserService userService) : ICommandHandler<DeactivateUserCommand>
{
    public async Task Handle(
        DeactivateUserCommand request,
        CancellationToken cancellationToken)
    {
        await userService.DeactivateAsync(
            request.UserId,
            cancellationToken);
    }
}
