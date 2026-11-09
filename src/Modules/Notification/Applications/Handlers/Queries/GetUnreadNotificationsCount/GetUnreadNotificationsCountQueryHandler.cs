namespace NoMoreTears.Modules.Notification.Applications.Handlers.Queries.GetUnreadNotificationsCount;

public sealed class GetUnreadNotificationsCountQueryHandler(
    INotificationService notificationService) : IQueryHandler<GetUnreadNotificationsCountQuery, UnreadCountResponse>
{
    public async Task<UnreadCountResponse> Handle(
        GetUnreadNotificationsCountQuery request,
        CancellationToken cancellationToken)
    {
        return await notificationService.GetUnreadCountAsync(request.RecipientId, cancellationToken);
    }
}
