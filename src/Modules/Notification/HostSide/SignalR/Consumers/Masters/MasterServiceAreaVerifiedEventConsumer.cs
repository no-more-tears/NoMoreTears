using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Masters;

/// <summary>
///     Notifies the master about <see cref="MasterServiceAreaVerifiedEvent"/>.
/// </summary>
public sealed class MasterServiceAreaVerifiedEventConsumer(
    ISender sender) : IEventConsumer<MasterServiceAreaVerifiedEvent>
{
    public async Task Consume(ConsumeContext<MasterServiceAreaVerifiedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.MasterId,
            message.EventId,
            NotificationTypes.ServiceAreaVerified,
            NotificationData.Of(("masterId", message.MasterId)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}
