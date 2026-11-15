using MassTransit;
using MediatR;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Modules.Requests.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Requests;

/// <summary>
///     Asks the client to confirm the completion marked by the master.
/// </summary>
public sealed class RequestMarkedAsDoneEventConsumer(
    ISender sender) : IEventConsumer<RequestMarkedAsDoneEvent>
{
    public async Task Consume(ConsumeContext<RequestMarkedAsDoneEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.ClientId,
            message.EventId,
            NotificationTypes.RequestMarkedAsDone,
            NotificationData.Of(
                ("requestId", message.RequestId),
                ("masterId", message.MasterId),
                ("confirmationDeadline", message.ConfirmationDeadline)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}