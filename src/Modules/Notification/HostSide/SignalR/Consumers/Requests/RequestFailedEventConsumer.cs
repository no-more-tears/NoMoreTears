using MassTransit;
using MediatR;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Modules.Requests.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Requests;

/// <summary>
///     Notifies the other party about a failure: the master about a reported no-show, the client about a refusal.
/// </summary>
public sealed class RequestFailedEventConsumer(
    ISender sender) : IEventConsumer<RequestFailedEvent>
{
    public async Task Consume(ConsumeContext<RequestFailedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var recipientId = message.Kind == "NoShow" ? message.MasterId : message.ClientId;
        var command = new CreateNotificationCommand(
            recipientId,
            message.EventId,
            NotificationTypes.RequestFailed,
            NotificationData.Of(
                ("requestId", message.RequestId),
                ("kind", message.Kind),
                ("reason", message.Reason)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}