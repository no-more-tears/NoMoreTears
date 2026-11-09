using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ApplySanction;
using NoMoreTears.Modules.Trust.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Masters.HostSide.Consumers.Consumers.Trust;

/// <summary>
///     Applies a sanction of the ladder imposed by Trust on a master (FR-MST-06).
/// </summary>
public sealed class SanctionImposedEventConsumer(
    ISender sender) : IEventConsumer<SanctionImposedEvent>
{
    public async Task Consume(ConsumeContext<SanctionImposedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        if (!TrustEventFilters.IsAboutMaster(message.SubjectRole) || !TrustEventFilters.IsMasterSanction(message.Kind))
        {
            return;
        }

        var command = new ApplySanctionCommand(
            message.SubjectId,
            message.SanctionId,
            message.Kind,
            message.ImposedAt,
            message.ExpiresAt);

        await sender.Send(command, context.CancellationToken);
    }
}
