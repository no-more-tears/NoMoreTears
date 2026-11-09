using MassTransit;
using MediatR;
using NoMoreTears.Modules.Identity.Contracts.Events;
using NoMoreTears.Modules.Notification.Applications.Services.Emails;
using NoMoreTears.Shared.RabbitMq.Abstractions;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers.Identity;

/// <summary>
///     Sends the password reset link requested by Identity.
/// </summary>
public sealed class PasswordResetRequestedEventConsumer(
    ISender sender) : IEventConsumer<PasswordResetRequestedEvent>
{
    public Task Consume(ConsumeContext<PasswordResetRequestedEvent> context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var message = context.Message;

        return EmailConsumption.SendAsync(
            sender,
            context,
            message.EventId,
            message.UserId,
            message.Email,
            EmailTemplates.PasswordReset,
            EmailModels.WithLink(message.Email, message.ResetUrl, message.ExpiresAt));
    }
}
