using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Masters;

/// <summary>
///     Notifies the master about <see cref="MasterVerificationRestoredEvent"/>.
/// </summary>
public sealed class MasterVerificationRestoredEventConsumer(
    ISender sender) : IEventConsumer<MasterVerificationRestoredEvent>
{
    public async Task Consume(ConsumeContext<MasterVerificationRestoredEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.MasterId,
            message.EventId,
            NotificationTypes.VerificationRestored,
            NotificationData.Of(("masterId", message.MasterId), ("sanctionId", message.SanctionId)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}
