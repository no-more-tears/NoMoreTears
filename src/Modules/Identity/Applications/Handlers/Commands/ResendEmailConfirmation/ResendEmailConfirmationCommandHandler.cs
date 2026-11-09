using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Modules.Identity.Applications.Services.Messaging;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.ResendEmailConfirmation;

public sealed class ResendEmailConfirmationCommandHandler(
    IIdentityService identityService,
    IEventPublisher<IIdentityOutbox> eventPublisher) : ICommandHandler<ResendEmailConfirmationCommand>
{
    public async Task Handle(
        ResendEmailConfirmationCommand request,
        CancellationToken cancellationToken)
    {
        var emailConfirmation = await identityService.ResendEmailConfirmationAsync(
            request.Identifier,
            cancellationToken);

        if (emailConfirmation is not null)
        {
            await eventPublisher.PublishAsync(emailConfirmation, cancellationToken);
        }
    }
}
