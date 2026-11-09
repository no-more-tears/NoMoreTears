namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.SendEmail;

public sealed class SendEmailCommandHandler(
    IEmailDeliveryService emailDeliveryService) : ICommandHandler<SendEmailCommand>
{
    public async Task Handle(
        SendEmailCommand request,
        CancellationToken cancellationToken)
    {
        await emailDeliveryService.SendAsync(
            request.SourceEventId,
            request.RecipientId,
            request.Address,
            request.Template,
            request.Model,
            request.Attempt,
            cancellationToken);
    }
}
