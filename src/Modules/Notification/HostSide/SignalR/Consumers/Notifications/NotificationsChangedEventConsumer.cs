using MassTransit;
using MediatR;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.PushNotificationsChange;
using NoMoreTears.Modules.Notification.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Notifications;

/// <summary>
///     Pushes notifications read or deleted over HTTP to the other connected clients of the recipient.
/// </summary>
public sealed class NotificationsChangedEventConsumer(
    ISender sender) : IEventConsumer<NotificationsChangedEvent>
{
    public async Task Consume(ConsumeContext<NotificationsChangedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new PushNotificationsChangeCommand(
            message.RecipientId,
            message.Change,
            message.NotificationIds,
            message.AffectedCount,
            message.UnreadCount,
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}
