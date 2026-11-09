using MassTransit;
using MediatR;
using NoMoreTears.Modules.Identity.Contracts.Events;
using NoMoreTears.Modules.Notification.Applications.Services.Emails;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Identity;

/// <summary>
///     Sends the one-time sign-in code issued by Identity.
/// </summary>
public sealed class MfaCodeIssuedEventConsumer(
    ISender sender) : IEventConsumer<MfaCodeIssuedEvent>
{
    public Task Consume(ConsumeContext<MfaCodeIssuedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        return EmailConsumption.SendAsync(
            sender,
            context,
            message.EventId,
            message.UserId,
            message.Email,
            EmailTemplates.MfaCode,
            EmailModels.WithCode(message.Email, message.Code, message.ExpiresAt));
    }
}
