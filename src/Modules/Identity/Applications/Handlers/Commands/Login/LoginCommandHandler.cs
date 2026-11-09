using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Modules.Identity.Applications.Services.Messaging;
using NoMoreTears.Modules.Identity.Contracts.Responses;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Login;

public sealed class LoginCommandHandler(
    IIdentityService identityService,
    IEventPublisher<IIdentityOutbox> eventPublisher) : ICommandHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var login = await identityService.LoginAsync(
            request.Identifier,
            request.Password,
            request.Context,
            cancellationToken);

        if (login.MfaCode is not null)
        {
            await eventPublisher.PublishAsync(login.MfaCode, cancellationToken);
        }

        return login.Response;
    }
}
