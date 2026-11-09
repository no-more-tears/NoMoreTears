using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Masters;

/// <summary>
///     Notifies the master about <see cref="MasterSanctionLiftedEvent"/>.
/// </summary>
public sealed class MasterSanctionLiftedEventConsumer(
    ISender sender) : IEventConsumer<MasterSanctionLiftedEvent>
{
    public async Task Consume(ConsumeContext<MasterSanctionLiftedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.MasterId,
            message.EventId,
            NotificationTypes.SanctionLifted,
            NotificationData.Of(("masterId", message.MasterId), ("sanctionId", message.SanctionId), ("kind", message.Kind)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}
