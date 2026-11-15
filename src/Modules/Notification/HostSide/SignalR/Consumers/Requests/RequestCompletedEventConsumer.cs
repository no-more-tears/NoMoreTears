using MassTransit;
using MediatR;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Modules.Requests.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Requests;

/// <summary>
///     Notifies the master about the completion; the client is notified only about an automatic completion.
/// </summary>
public sealed class RequestCompletedEventConsumer(
    ISender sender) : IEventConsumer<RequestCompletedEvent>
{
    public async Task Consume(ConsumeContext<RequestCompletedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var data = NotificationData.Of(
            ("requestId", message.RequestId),
            ("completedAt", message.CompletedAt));

        await sender.Send(
            new CreateNotificationCommand(message.MasterId, message.EventId, NotificationTypes.RequestCompleted, data, message.OccurredAt),
            context.CancellationToken);

        if (!message.IsConfirmedByClient)
        {
            await sender.Send(
                new CreateNotificationCommand(message.ClientId, message.EventId, NotificationTypes.RequestCompleted, data, message.OccurredAt),
                context.CancellationToken);
        }
    }
}