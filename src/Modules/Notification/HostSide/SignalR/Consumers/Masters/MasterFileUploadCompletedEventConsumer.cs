using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Masters;

/// <summary>
///     Notifies the master about <see cref="MasterFileUploadCompletedEvent"/>.
/// </summary>
public sealed class MasterFileUploadCompletedEventConsumer(
    ISender sender) : IEventConsumer<MasterFileUploadCompletedEvent>
{
    public async Task Consume(ConsumeContext<MasterFileUploadCompletedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.MasterId,
            message.EventId,
            NotificationTypes.UploadCompleted,
            NotificationData.Of(("masterId", message.MasterId), ("uploadId", message.UploadId), ("purpose", message.Purpose), ("fileName", message.FileName), ("categoryId", message.CategoryId), ("documentId", message.DocumentId)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}
