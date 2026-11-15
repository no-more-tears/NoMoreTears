using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.SaveCategorySnapshot;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Masters;

/// <summary>
///     Adds a category of the Masters catalog to the local snapshot (FR-MST-07).
/// </summary>
public sealed class CategoryCreatedEventConsumer(
    ISender sender) : IEventConsumer<CategoryCreatedEvent>
{
    public async Task Consume(ConsumeContext<CategoryCreatedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        await sender.Send(new SaveCategorySnapshotCommand(message.CategoryId, message.Name, message.OccurredAt), context.CancellationToken);
    }
}