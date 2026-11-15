using MassTransit;
using MediatR;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Modules.Requests.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Requests;

/// <summary>
///     Notifies the client that a master accepted the request, with the expected arrival (FR-NTF-02).
/// </summary>
public sealed class RequestAcceptedEventConsumer(
    ISender sender) : IEventConsumer<RequestAcceptedEvent>
{
    public async Task Consume(ConsumeContext<RequestAcceptedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.ClientId,
            message.EventId,
            NotificationTypes.RequestAccepted,
            NotificationData.Of(
                ("requestId", message.RequestId),
                ("masterId", message.MasterId),
                ("expectedArrivalAt", message.ExpectedArrivalAt)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}