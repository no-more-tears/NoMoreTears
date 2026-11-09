namespace NoMoreTears.Modules.Notification.Applications.Handlers.Queries.GetUnreadNotificationsCount;

public sealed record GetUnreadNotificationsCountQuery(
    Guid RecipientId) : IQuery<UnreadCountResponse>;
