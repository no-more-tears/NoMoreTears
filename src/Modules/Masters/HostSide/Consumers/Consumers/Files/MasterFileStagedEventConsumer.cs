using MassTransit;
using MediatR;
using NoMoreTears.Modules.Masters.Applications.Handlers.Commands.ProcessFileUpload;
using NoMoreTears.Modules.Masters.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Masters.HostSide.Consumers.Consumers.Files;

/// <summary>
///     Checks a staged file and attaches it to the master profile.
/// </summary>
public sealed class MasterFileStagedEventConsumer(
    ISender sender) : IEventConsumer<MasterFileStagedEvent>
{
    public async Task Consume(ConsumeContext<MasterFileStagedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        await sender.Send(new ProcessFileUploadCommand(context.Message.UploadId), context.CancellationToken);
    }
}
