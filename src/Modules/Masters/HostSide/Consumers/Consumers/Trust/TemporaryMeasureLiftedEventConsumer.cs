using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.LiftTemporaryMeasure;
using NoMoreTears.Modules.Trust.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Masters.HostSide.Consumers.Consumers.Trust;

/// <summary>
///     Removes the Masters effects of a temporary measure that was lifted or expired.
/// </summary>
public sealed class TemporaryMeasureLiftedEventConsumer(
    ISender sender) : IEventConsumer<TemporaryMeasureLiftedEvent>
{
    public async Task Consume(ConsumeContext<TemporaryMeasureLiftedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var suspendsFeed = TrustEventFilters.SuspendsFeed(message.Effects);
        var holdsVerification = TrustEventFilters.HoldsVerification(message.Effects);

        if (!TrustEventFilters.IsAboutMaster(message.SubjectRole) || (!suspendsFeed && !holdsVerification))
        {
            return;
        }

        var command = new LiftTemporaryMeasureCommand(
            message.SubjectId,
            message.MeasureId,
            suspendsFeed,
            holdsVerification,
            message.LiftedAt);

        await sender.Send(command, context.CancellationToken);
    }
}
