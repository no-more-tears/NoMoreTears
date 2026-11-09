using System.Collections.Frozen;
using NoMoreTears.Modules.Notification.Contracts.Constants;
using NoMoreTears.Modules.Notification.Domain.Enums;

namespace NoMoreTears.Modules.Notification.Applications.Services.Notifications;

/// <summary>
///     Definition of a kind of in-app notification.
/// </summary>
/// <param name="Category">Area of the platform.</param>
/// <param name="Severity">Importance for the recipient.</param>
/// <param name="Link">Relative route of the client that opens the subject.</param>
public sealed record NotificationDefinition(
    NotificationCategory Category,
    NotificationSeverity Severity,
    string? Link);

/// <summary>
///     Known kinds of in-app notifications. Texts live in <c>Templates/Notifications/{culture}.json</c>.
/// </summary>
public static class NotificationCatalog
{
    private const string ProfileLink = "/master/profile";
    private const string VerificationLink = "/master/profile/verification";
    private const string DocumentsLink = "/master/profile/documents";
    private const string SanctionsLink = "/master/profile/sanctions";

    private static readonly FrozenDictionary<string, NotificationDefinition> Definitions =
        new Dictionary<string, NotificationDefinition>(StringComparer.Ordinal)
        {
            [NotificationTypes.VerificationSubmitted] = new(NotificationCategory.Verification, NotificationSeverity.Info, VerificationLink),
            [NotificationTypes.VerificationVerified] = new(NotificationCategory.Verification, NotificationSeverity.Success, VerificationLink),
            [NotificationTypes.VerificationRejected] = new(NotificationCategory.Verification, NotificationSeverity.Warning, VerificationLink),
            [NotificationTypes.VerificationRevoked] = new(NotificationCategory.Sanction, NotificationSeverity.Critical, SanctionsLink),
            [NotificationTypes.VerificationRestored] = new(NotificationCategory.Sanction, NotificationSeverity.Success, VerificationLink),
            [NotificationTypes.CategoryVerified] = new(NotificationCategory.Verification, NotificationSeverity.Success, VerificationLink),
            [NotificationTypes.CategoryRejected] = new(NotificationCategory.Verification, NotificationSeverity.Warning, VerificationLink),
            [NotificationTypes.ServiceAreaVerified] = new(NotificationCategory.Verification, NotificationSeverity.Success, VerificationLink),
            [NotificationTypes.ServiceAreaRejected] = new(NotificationCategory.Verification, NotificationSeverity.Warning, VerificationLink),
            [NotificationTypes.VerificationHoldPlaced] = new(NotificationCategory.Verification, NotificationSeverity.Warning, VerificationLink),
            [NotificationTypes.VerificationHoldReleased] = new(NotificationCategory.Verification, NotificationSeverity.Info, VerificationLink),
            [NotificationTypes.SanctionApplied] = new(NotificationCategory.Sanction, NotificationSeverity.Critical, SanctionsLink),
            [NotificationTypes.SanctionLifted] = new(NotificationCategory.Sanction, NotificationSeverity.Info, SanctionsLink),
            [NotificationTypes.UploadCompleted] = new(NotificationCategory.Profile, NotificationSeverity.Success, DocumentsLink),
            [NotificationTypes.UploadFailed] = new(NotificationCategory.Profile, NotificationSeverity.Warning, ProfileLink)
        }.ToFrozenDictionary(StringComparer.Ordinal);

    /// <summary>
    ///     Returns the definition of a known type.
    /// </summary>
    public static NotificationDefinition Get(string type)
    {
        return Definitions.TryGetValue(type, out var definition)
            ? definition
            : throw new ArgumentException($"Notification type '{type}' is unknown.", nameof(type));
    }
}