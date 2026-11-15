using MassTransit;
using MediatR;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ApplyClientRestriction;
using NoMoreTears.Modules.Trust.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Trust;

/// <summary>
///     Applies a sanction of the ladder that restricts publication of a client (steps 2 and 3, FR-REQ-09).
/// </summary>
public sealed class SanctionImposedEventConsumer(
    ISender sender) : IEventConsumer<SanctionImposedEvent>
{
    public async Task Consume(ConsumeContext<SanctionImposedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        if (!TrustEventFilters.IsAboutClient(message.SubjectRole)
            || TrustEventFilters.RestrictionOf(message.Kind) is not { } restrictionKind)
        {
            return;
        }

        var command = new ApplyClientRestrictionCommand(
            message.SanctionId,
            message.SubjectId,
            restrictionKind,
            message.ImposedAt,
            message.ExpiresAt);

        await sender.Send(command, context.CancellationToken);
    }
}