using MassTransit;
using MediatR;
using NoMoreTears.Modules.Requests.Applications.Handlers.Commands.ProcessMediaUpload;
using NoMoreTears.Modules.Requests.Contracts.Events;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Requests.HostSide.Consumers.Consumers.Files;

/// <summary>
///     Checks a staged photo and attaches it to the draft.
/// </summary>
public sealed class RequestMediaStagedEventConsumer(
    ISender sender) : IEventConsumer<RequestMediaStagedEvent>
{
    public async Task Consume(ConsumeContext<RequestMediaStagedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        await sender.Send(new ProcessMediaUploadCommand(context.Message.UploadId), context.CancellationToken);
    }
}