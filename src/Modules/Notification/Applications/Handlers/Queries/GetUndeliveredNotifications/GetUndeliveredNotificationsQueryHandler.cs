namespace NoMoreTears.Modules.Notification.Applications.Handlers.Queries.GetUndeliveredNotifications;

public sealed class GetUndeliveredNotificationsQueryHandler(
    INotificationService notificationService) : IQueryHandler<GetUndeliveredNotificationsQuery, IReadOnlyList<NotificationResponse>>
{
    public async Task<IReadOnlyList<NotificationResponse>> Handle(
        GetUndeliveredNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        return await notificationService.GetUndeliveredAsync(request.RecipientId, cancellationToken);
    }
}
