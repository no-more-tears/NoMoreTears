using MassTransit;
using MediatR;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DisputeRequest;
using NoMoreTears.Modules.Trust.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Trust;

/// <summary>
///     Moves a request to dispute when Trust accepted a complaint about it (FR-TRS-04).
/// </summary>
public sealed class IncidentOpenedEventConsumer(
    ISender sender) : IEventConsumer<IncidentOpenedEvent>
{
    public async Task Consume(ConsumeContext<IncidentOpenedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        if (message.RequestId is not { } requestId || !TrustEventFilters.IsComplaint(message.Kind))
        {
            return;
        }

        await sender.Send(new DisputeRequestCommand(requestId, message.IncidentId), context.CancellationToken);
    }
}