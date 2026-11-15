using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ActivateCategorySnapshot;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Masters;

/// <summary>
///     Activates a category in the local snapshot of the catalog.
/// </summary>
public sealed class CategoryActivatedEventConsumer(
    ISender sender) : IEventConsumer<CategoryActivatedEvent>
{
    public async Task Consume(ConsumeContext<CategoryActivatedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        await sender.Send(new ActivateCategorySnapshotCommand(message.CategoryId, message.OccurredAt), context.CancellationToken);
    }
}