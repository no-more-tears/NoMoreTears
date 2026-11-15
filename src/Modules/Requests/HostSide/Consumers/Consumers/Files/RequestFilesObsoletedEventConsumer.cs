using MassTransit;
using MediatR;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.DeleteObsoleteFiles;
using NoMoreTears.Modules.Requests.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Files;

/// <summary>
///     Removes stored files that are no longer referenced, after the change that released them was committed.
/// </summary>
public sealed class RequestFilesObsoletedEventConsumer(
    ISender sender) : IEventConsumer<RequestFilesObsoletedEvent>
{
    public async Task Consume(ConsumeContext<RequestFilesObsoletedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        await sender.Send(new DeleteObsoleteFilesCommand(context.Message.Files), context.CancellationToken);
    }
}