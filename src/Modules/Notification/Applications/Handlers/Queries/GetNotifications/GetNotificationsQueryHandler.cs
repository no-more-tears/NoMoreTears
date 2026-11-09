using NoMoreTears.Shared.Common.Pagination;

namespace NoMoreTears.Modules.Notification.Applications.Handlers.Queries.GetNotifications;

public sealed class GetNotificationsQueryHandler(
    INotificationService notificationService) : IQueryHandler<GetNotificationsQuery, PagedResponse<NotificationResponse>>
{
    public async Task<PagedResponse<NotificationResponse>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        return await notificationService.GetPageAsync(request.RecipientId, request.UnreadOnly, request.Category, request.PageNumber, request.PageSize, cancellationToken);
    }
}
