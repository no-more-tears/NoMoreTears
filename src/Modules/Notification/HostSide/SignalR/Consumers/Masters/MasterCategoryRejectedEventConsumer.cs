using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Masters;

/// <summary>
///     Notifies the master about <see cref="MasterCategoryRejectedEvent"/>.
/// </summary>
public sealed class MasterCategoryRejectedEventConsumer(
    ISender sender) : IEventConsumer<MasterCategoryRejectedEvent>
{
    public async Task Consume(ConsumeContext<MasterCategoryRejectedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.MasterId,
            message.EventId,
            NotificationTypes.CategoryRejected,
            NotificationData.Of(("masterId", message.MasterId), ("categoryId", message.CategoryId), ("reason", message.Reason)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}
