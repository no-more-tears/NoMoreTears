using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ApplyTemporaryMeasure;
using NoMoreTears.Modules.Trust.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Masters.HostSide.Consumers.Consumers.Trust;

/// <summary>
///     Applies the Masters effects of an automatic temporary measure (FR-TRS-07, FR-MST-09):
///     feed suspension and verification hold until a human check.
/// </summary>
public sealed class TemporaryMeasureAppliedEventConsumer(
    ISender sender) : IEventConsumer<TemporaryMeasureAppliedEvent>
{
    public async Task Consume(ConsumeContext<TemporaryMeasureAppliedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;
        var suspendsFeed = TrustEventFilters.SuspendsFeed(message.Effects);
        var holdsVerification = TrustEventFilters.HoldsVerification(message.Effects);

        if (!TrustEventFilters.IsAboutMaster(message.SubjectRole) || (!suspendsFeed && !holdsVerification))
        {
            return;
        }

        var command = new ApplyTemporaryMeasureCommand(
            message.SubjectId,
            message.MeasureId,
            message.Rule,
            suspendsFeed,
            holdsVerification,
            message.AppliedAt,
            message.ExpiresAt);

        await sender.Send(command, context.CancellationToken);
    }
}
