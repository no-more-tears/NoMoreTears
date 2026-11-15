using MassTransit;
using MediatR;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.LiftClientRestriction;
using NoMoreTears.Modules.Trust.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Trust;

/// <summary>
///     Lifts a sanction that restricted publication of a client.
/// </summary>
public sealed class SanctionLiftedEventConsumer(
    ISender sender) : IEventConsumer<SanctionLiftedEvent>
{
    public async Task Consume(ConsumeContext<SanctionLiftedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        if (!TrustEventFilters.IsAboutClient(message.SubjectRole) || TrustEventFilters.RestrictionOf(message.Kind) is null)
        {
            return;
        }

        await sender.Send(new LiftClientRestrictionCommand(message.SanctionId, message.LiftedAt), context.CancellationToken);
    }
}