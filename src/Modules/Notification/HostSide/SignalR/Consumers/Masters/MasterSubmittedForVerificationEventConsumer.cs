using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.CreateNotification;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Masters;

/// <summary>
///     Notifies the master about <see cref="MasterSubmittedForVerificationEvent"/>.
/// </summary>
public sealed class MasterSubmittedForVerificationEventConsumer(
    ISender sender) : IEventConsumer<MasterSubmittedForVerificationEvent>
{
    public async Task Consume(ConsumeContext<MasterSubmittedForVerificationEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var command = new CreateNotificationCommand(
            message.MasterId,
            message.EventId,
            NotificationTypes.VerificationSubmitted,
            NotificationData.Of(("masterId", message.MasterId)),
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}
