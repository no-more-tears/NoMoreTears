using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.DeleteObsoleteFiles;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Masters.HostSide.Consumers.Consumers.Files;

/// <summary>
///     Removes stored files that are no longer referenced, after the change that released them was committed.
/// </summary>
public sealed class MasterFilesObsoletedEventConsumer(
    ISender sender) : IEventConsumer<MasterFilesObsoletedEvent>
{
    public async Task Consume(ConsumeContext<MasterFilesObsoletedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        await sender.Send(new DeleteObsoleteFilesCommand(context.Message.Files), context.CancellationToken);
    }
}
