using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ArchiveCategorySnapshot;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Masters;

/// <summary>
///     Archives a category in the local snapshot: it can no longer be chosen for requests.
/// </summary>
public sealed class CategoryArchivedEventConsumer(
    ISender sender) : IEventConsumer<CategoryArchivedEvent>
{
    public async Task Consume(ConsumeContext<CategoryArchivedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        await sender.Send(new ArchiveCategorySnapshotCommand(message.CategoryId, message.OccurredAt), context.CancellationToken);
    }
}