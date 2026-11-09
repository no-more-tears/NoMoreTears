using MassTransit;
using MediatR;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.PushNotification;
using NoMoreTears.Modules.Notification.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Notifications;

/// <summary>
///     Pushes a committed notification to the connected clients of its recipient.
///     Offline clients get it from the hub on the next connection.
/// </summary>
public sealed class NotificationCreatedEventConsumer(
    ISender sender) : IEventConsumer<NotificationCreatedEvent>
{
    public async Task Consume(ConsumeContext<NotificationCreatedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        await sender.Send(new PushNotificationCommand(context.Message.NotificationId), context.CancellationToken);
    }
}