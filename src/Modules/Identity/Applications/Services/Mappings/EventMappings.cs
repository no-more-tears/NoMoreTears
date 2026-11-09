using NoMoreTears.Modules.Identity.Applications.Services.Configuration;
using NoMoreTears.Modules.Identity.Contracts.Events;
using NoMoreTears.Shared.Providers.Identity.Contracts.SignIn;
using NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;

namespace NoMoreTears.Modules.Identity.Applications.Services.Mappings;

/// <summary>
///     Maps identity tickets to the events that ask Notification to deliver them.
/// </summary>
public static class EventMappings
{
    public static EmailConfirmationRequestedEvent ToEmailConfirmationRequestedEvent(
        this UserActionTicket ticket,
        string urlTemplate,
        DateTimeOffset occurredAt)
    {
        EnsurePurpose(ticket, UserActionPurpose.EmailConfirmation);

        return new EmailConfirmationRequestedEvent(
            ticket.UserId,
            ticket.Email,
            IdentityLinks.Build(urlTemplate, ticket),
            ticket.ExpiresAt,
            occurredAt);
    }

    public static PasswordResetRequestedEvent ToPasswordResetRequestedEvent(
        this UserActionTicket ticket,
        string urlTemplate,
        DateTimeOffset occurredAt)
    {
        EnsurePurpose(ticket, UserActionPurpose.PasswordReset);

        return new PasswordResetRequestedEvent(
            ticket.UserId,
            ticket.Email,
            IdentityLinks.Build(urlTemplate, ticket),
            ticket.ExpiresAt,
            occurredAt);
    }

    public static MfaCodeIssuedEvent ToMfaCodeIssuedEvent(
        this MfaChallenge challenge,
        DateTimeOffset occurredAt)
    {
        ArgumentNullException.ThrowIfNull(challenge);

        return new MfaCodeIssuedEvent(
            challenge.UserId,
            challenge.Email,
            challenge.Code,
            challenge.ExpiresAt,
            occurredAt);
    }

    private static void EnsurePurpose(
        UserActionTicket ticket,
        UserActionPurpose purpose)
    {
        ArgumentNullException.ThrowIfNull(ticket);

        if (ticket.Purpose != purpose)
        {
            throw new ArgumentException($"Ticket purpose must be {purpose}.", nameof(ticket));
        }
    }
}
