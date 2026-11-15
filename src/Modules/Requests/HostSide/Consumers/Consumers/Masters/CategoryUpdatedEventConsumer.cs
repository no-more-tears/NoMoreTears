using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.SaveCategorySnapshot;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Masters;

/// <summary>
///     Renames a category in the local snapshot of the catalog.
/// </summary>
public sealed class CategoryUpdatedEventConsumer(
    ISender sender) : IEventConsumer<CategoryUpdatedEvent>
{
    public async Task Consume(ConsumeContext<CategoryUpdatedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        await sender.Send(new SaveCategorySnapshotCommand(message.CategoryId, message.Name, message.OccurredAt), context.CancellationToken);
    }
}