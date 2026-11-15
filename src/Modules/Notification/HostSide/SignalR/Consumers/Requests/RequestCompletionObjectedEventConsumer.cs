using MassTransit;
using MediatR;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Modules.Requests.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Requests;

/// <summary>
///     Notifies the master that the client objected to the completion.
/// </summary>
public sealed class RequestCompletionObjectedEventConsumer(
    ISender sender) : IEventConsumer<RequestCompletionObjectedEvent>
{
    public async Task Consume(ConsumeContext<RequestCompletionObjectedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.MasterId,
            message.EventId,
            NotificationTypes.RequestCompletionObjected,
            NotificationData.Of(
                ("requestId", message.RequestId),
                ("reason", message.Reason)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}