using MassTransit;
using MediatR;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ApplyClientRestriction;
using NoMoreTears.Modules.Trust.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Trust;

/// <summary>
///     Applies a temporary measure that limits a client to one published request (client abuse rule).
/// </summary>
public sealed class TemporaryMeasureAppliedEventConsumer(
    ISender sender) : IEventConsumer<TemporaryMeasureAppliedEvent>
{
    public async Task Consume(ConsumeContext<TemporaryMeasureAppliedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        if (!TrustEventFilters.IsAboutClient(message.SubjectRole) || !TrustEventFilters.LimitsPublication(message.Effects))
        {
            return;
        }

        var command = new ApplyClientRestrictionCommand(
            message.MeasureId,
            message.SubjectId,
            TrustEventFilters.PublicationLimit,
            message.AppliedAt,
            message.ExpiresAt);

        await sender.Send(command, context.CancellationToken);
    }
}