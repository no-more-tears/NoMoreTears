namespace NoMoreTears.Modules.Notification.Applications.Handlers.Commands.RecordEmailFailure;

public sealed class RecordEmailFailureCommandHandler(
    IEmailDeliveryService emailDeliveryService) : ICommandHandler<RecordEmailFailureCommand>
{
    public async Task Handle(
        RecordEmailFailureCommand request,
        CancellationToken cancellationToken)
    {
        await emailDeliveryService.RecordFailureAsync(request.SourceEventId, request.RecipientId, request.Template, request.Attempts, request.Error, cancellationToken);
    }
}
