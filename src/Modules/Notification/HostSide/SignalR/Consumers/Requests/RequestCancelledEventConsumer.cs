using MassTransit;
using MediatR;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Modules.Requests.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Requests;

/// <summary>
///     Notifies the master when the client cancelled a request after the master accepted it.
/// </summary>
public sealed class RequestCancelledEventConsumer(
    ISender sender) : IEventConsumer<RequestCancelledEvent>
{
    public async Task Consume(ConsumeContext<RequestCancelledEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        if (!message.IsAfterAcceptance || message.MasterId is not { } masterId)
        {
            return;
        }

        var command = new CreateNotificationCommand(
            masterId,
            message.EventId,
            NotificationTypes.RequestCancelled,
            NotificationData.Of(
                ("requestId", message.RequestId),
                ("reason", message.Reason)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}