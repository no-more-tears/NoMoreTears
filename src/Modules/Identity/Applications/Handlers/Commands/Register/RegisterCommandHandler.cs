using NoMoreTears.Modules.Identity.Applications.Services.Abstractions;
using NoMoreTears.Modules.Identity.Applications.Services.Messaging;
using NoMoreTears.Shared.CQS.Abstractions.Handlers;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Identity.Applications.Handlers.Commands.Register;

public sealed class RegisterCommandHandler(
    IIdentityService identityService,
    IEventPublisher<IIdentityOutbox> eventPublisher)
    : ICommandHandler<RegisterCommand, Guid>
{
    public async Task<Guid> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var registration = await identityService.RegisterAsync(
            request.Email,
            request.Username,
            request.Password,
            cancellationToken);

        await eventPublisher.PublishAsync(registration.EmailConfirmation, cancellationToken);

        return registration.UserId;
    }
}
