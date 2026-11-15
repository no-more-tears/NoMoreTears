using MassTransit;
using MediatR;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.SettleDispute;
using NoMoreTears.Modules.Trust.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Trust;

/// <summary>
///     Returns a disputed request to its previous status when Trust closed the incident.
/// </summary>
public sealed class IncidentClosedEventConsumer(
    ISender sender) : IEventConsumer<IncidentClosedEvent>
{
    public async Task Consume(ConsumeContext<IncidentClosedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        if (message.RequestId is not { } requestId)
        {
            return;
        }

        await sender.Send(new SettleDisputeCommand(requestId, message.IncidentId), context.CancellationToken);
    }
}