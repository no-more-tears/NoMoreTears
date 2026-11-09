using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Masters;

/// <summary>
///     Notifies the master about <see cref="MasterServiceAreaRejectedEvent"/>.
/// </summary>
public sealed class MasterServiceAreaRejectedEventConsumer(
    ISender sender) : IEventConsumer<MasterServiceAreaRejectedEvent>
{
    public async Task Consume(ConsumeContext<MasterServiceAreaRejectedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.MasterId,
            message.EventId,
            NotificationTypes.ServiceAreaRejected,
            NotificationData.Of(("masterId", message.MasterId), ("reason", message.Reason)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}
