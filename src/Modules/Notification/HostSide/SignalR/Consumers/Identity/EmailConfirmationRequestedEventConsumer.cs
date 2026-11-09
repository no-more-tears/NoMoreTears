using MassTransit;
using MediatR;
using NoMoreTears.Modules.Identity.Contracts.Events;
using NoMoreTears.Modules.Notification.Applications.Services.Emails;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Identity;

/// <summary>
///     Sends the email confirmation link requested by Identity.
/// </summary>
public sealed class EmailConfirmationRequestedEventConsumer(
    ISender sender) : IEventConsumer<EmailConfirmationRequestedEvent>
{
    public Task Consume(ConsumeContext<EmailConfirmationRequestedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        return EmailConsumption.SendAsync(
            sender,
            context,
            message.EventId,
            message.UserId,
            message.Email,
            EmailTemplates.EmailConfirmation,
            EmailModels.WithLink(message.Email, message.ConfirmationUrl, message.ExpiresAt));
    }
}