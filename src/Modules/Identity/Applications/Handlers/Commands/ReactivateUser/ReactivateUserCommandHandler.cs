using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ReactivateUser;

public sealed class ReactivateUserCommandHandler(
    IUserService userService) : ICommandHandler<ReactivateUserCommand>
{
    public async Task Handle(
        ReactivateUserCommand request,
        CancellationToken cancellationToken)
    {
        await userService.ReactivateAsync(
            request.UserId,
            cancellationToken);
    }
}
