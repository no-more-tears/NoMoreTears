using MassTransit;
using MediatR;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Modules.Requests.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Requests;

/// <summary>
///     Notifies the client that a photo was checked and attached to the request.
/// </summary>
public sealed class RequestMediaUploadCompletedEventConsumer(
    ISender sender) : IEventConsumer<RequestMediaUploadCompletedEvent>
{
    public async Task Consume(ConsumeContext<RequestMediaUploadCompletedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.ClientId,
            message.EventId,
            NotificationTypes.RequestMediaCompleted,
            NotificationData.Of(
                ("requestId", message.RequestId),
                ("uploadId", message.UploadId),
                ("mediaFileId", message.MediaFileId),
                ("fileName", message.FileName)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}