using MassTransit;
using MediatR;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Modules.Requests.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Requests;

/// <summary>
///     Notifies the client that a photo was rejected.
/// </summary>
public sealed class RequestMediaUploadFailedEventConsumer(
    ISender sender) : IEventConsumer<RequestMediaUploadFailedEvent>
{
    public async Task Consume(ConsumeContext<RequestMediaUploadFailedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.ClientId,
            message.EventId,
            NotificationTypes.RequestMediaFailed,
            NotificationData.Of(
                ("requestId", message.RequestId),
                ("uploadId", message.UploadId),
                ("fileName", message.FileName),
                ("reason", message.Reason)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}