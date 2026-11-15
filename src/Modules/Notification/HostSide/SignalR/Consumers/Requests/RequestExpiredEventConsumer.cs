using MassTransit;
using MediatR;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Modules.Requests.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Requests;

/// <summary>
///     Asks the client to extend or cancel a request nobody accepted (FR-REQ-03).
/// </summary>
public sealed class RequestExpiredEventConsumer(
    ISender sender) : IEventConsumer<RequestExpiredEvent>
{
    public async Task Consume(ConsumeContext<RequestExpiredEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.ClientId,
            message.EventId,
            NotificationTypes.RequestExpired,
            NotificationData.Of(
                ("requestId", message.RequestId),
                ("acceptanceDeadline", message.AcceptanceDeadline)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}