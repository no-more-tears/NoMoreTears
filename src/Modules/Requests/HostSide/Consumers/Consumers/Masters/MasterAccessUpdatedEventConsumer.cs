using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.SaveMasterAccess;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Masters;

/// <summary>
///     Replaces the local snapshot of the access of a master, used to check who may accept a request.
/// </summary>
public sealed class MasterAccessUpdatedEventConsumer(
    ISender sender) : IEventConsumer<MasterAccessUpdatedEvent>
{
    public async Task Consume(ConsumeContext<MasterAccessUpdatedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        var command = new SaveMasterAccessCommand(
            message.MasterId,
            MastersEventFilters.AcceptableCategoryIds(message),
            message.SuspendedUntil,
            message.OccurredAt);

        await sender.Send(command, context.CancellationToken);
    }
}