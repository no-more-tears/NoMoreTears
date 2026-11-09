using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Masters;

/// <summary>
///     Notifies the master about <see cref="MasterFileUploadFailedEvent"/>.
/// </summary>
public sealed class MasterFileUploadFailedEventConsumer(
    ISender sender) : IEventConsumer<MasterFileUploadFailedEvent>
{
    public async Task Consume(ConsumeContext<MasterFileUploadFailedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.MasterId,
            message.EventId,
            NotificationTypes.UploadFailed,
            NotificationData.Of(("masterId", message.MasterId), ("uploadId", message.UploadId), ("purpose", message.Purpose), ("fileName", message.FileName), ("reason", message.Reason)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}
