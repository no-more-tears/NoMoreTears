using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.LiftSanction;
using NoMoreTears.Modules.Trust.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Masters.HostSide.Consumers.Consumers.Trust;

/// <summary>
///     Lifts a sanction of a master, for example after an accepted appeal.
/// </summary>
public sealed class SanctionLiftedEventConsumer(
    ISender sender) : IEventConsumer<SanctionLiftedEvent>
{
    public async Task Consume(ConsumeContext<SanctionLiftedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        if (!TrustEventFilters.IsAboutMaster(message.SubjectRole) || !TrustEventFilters.IsMasterSanction(message.Kind))
        {
            return;
        }

        await sender.Send(
            new LiftSanctionCommand(message.SubjectId, message.SanctionId, message.LiftedAt),
            context.CancellationToken);
    }
}
