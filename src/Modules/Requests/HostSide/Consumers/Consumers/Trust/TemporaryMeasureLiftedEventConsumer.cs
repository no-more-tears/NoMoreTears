using MassTransit;
using MediatR;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.LiftClientRestriction;
using NoMoreTears.Modules.Trust.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Trust;

/// <summary>
///     Lifts a temporary measure that limited publication of a client.
/// </summary>
public sealed class TemporaryMeasureLiftedEventConsumer(
    ISender sender) : IEventConsumer<TemporaryMeasureLiftedEvent>
{
    public async Task Consume(ConsumeContext<TemporaryMeasureLiftedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        if (!TrustEventFilters.IsAboutClient(message.SubjectRole) || !TrustEventFilters.LimitsPublication(message.Effects))
        {
            return;
        }

        await sender.Send(new LiftClientRestrictionCommand(message.MeasureId, message.LiftedAt), context.CancellationToken);
    }
}