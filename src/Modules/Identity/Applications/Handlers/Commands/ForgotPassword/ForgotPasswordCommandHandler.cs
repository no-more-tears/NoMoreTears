using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Modules.Identity.Applications.Services.Messaging;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IIdentityService identityService,
    IEventPublisher<IIdentityOutbox> eventPublisher) : ICommandHandler<ForgotPasswordCommand>
{
    public async Task Handle(
        ForgotPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var passwordReset = await identityService.ForgotPasswordAsync(
            request.Identifier,
            cancellationToken);

        if (passwordReset is not null)
        {
            await eventPublisher.PublishAsync(passwordReset, cancellationToken);
        }
    }
}
