using MassTransit;
using MediatR;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.RecordEmailFailure;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.SendEmail;
using NoMoreTears.Shared.RabbitMq.Registration;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Identity;

/// <summary>
///     Sends an email with retries: failed attempts are retried with exponential delay; after the last retry the
///     failure is recorded as the delivery status instead of moving the message to the error queue (FR-NTF-03).
/// </summary>
internal static class EmailConsumption
{
    public static async Task SendAsync<TMessage>(
        ISender sender,
        ConsumeContext<TMessage> context,
        Guid sourceEventId,
        Guid recipientId,
        string address,
        string template,
        IReadOnlyDictionary<string, string> model)
        where TMessage : class
    {
        var attempt = context.GetRetryAttempt() + 1;

        try
        {
            await sender.Send(
                new SendEmailCommand(sourceEventId, recipientId, address, template, model, attempt),
                context.CancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException && attempt > RabbitMqRegistrar.RetryLimit)
        {
            await sender.Send(
                new RecordEmailFailureCommand(sourceEventId, recipientId, template, attempt, ex.Message),
                context.CancellationToken);
        }
    }
}
