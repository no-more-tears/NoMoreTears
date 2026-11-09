using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using NoMoreTears.Modules.Notification.Applications.Handlers.Commands.AcknowledgeNotifications;
using NoMoreTears.Modules.Notification.Applications.Handlers.Queries.GetUndeliveredNotifications;
using NoMoreTears.Modules.Notification.Applications.Handlers.Queries.GetUnreadNotificationsCount;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Hubs;

/// <summary>
///     Real-time channel of in-app notifications. Redis backplane.
/// </summary>
[Authorize]
public sealed class NotificationsHub(
    ISender sender) : Hub<INotificationsClient>
{
    /// <summary>
    ///     Sends the notifications accumulated while the user was offline and the unread count.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var recipientId = GetRecipientId();
        var cancellationToken = Context.ConnectionAborted;

        var undelivered = await sender.Send(new GetUndeliveredNotificationsQuery(recipientId), cancellationToken);

        if (undelivered.Count > 0)
        {
            await Clients.Caller.ReceiveNotifications(undelivered);
        }

        var unreadCount = await sender.Send(new GetUnreadNotificationsCountQuery(recipientId), cancellationToken);
        await Clients.Caller.UnreadCountChanged(unreadCount);

        await base.OnConnectedAsync();
    }

    /// <summary>
    ///     Confirms that the client received notifications; unconfirmed ones are sent again on the next connection.
    /// </summary>
    public async Task<int> Acknowledge(Guid[] notificationIds)
    {
        return await sender.Send(
            new AcknowledgeNotificationsCommand(GetRecipientId(), notificationIds),
            Context.ConnectionAborted);
    }

    private Guid GetRecipientId()
    {
        return Guid.TryParse(Context.UserIdentifier, out var userId)
            ? userId
            : throw new HubException("The user is not authenticated.");
    }
}