namespace NoMoreTears.Modules.Notification.Contracts.Constants;

/// <summary>
///     Stable codes of in-app notifications. Clients may render their own texts and actions by these codes;
///     the values of <c>Data</c> for each code are listed in the documentation of the hub.
/// </summary>
public static class NotificationTypes
{
    public const string VerificationSubmitted = "masters.verification.submitted";

    public const string VerificationVerified = "masters.verification.verified";

    public const string VerificationRejected = "masters.verification.rejected";

    public const string VerificationRevoked = "masters.verification.revoked";

    public const string VerificationRestored = "masters.verification.restored";

    public const string CategoryVerified = "masters.category.verified";

    public const string CategoryRejected = "masters.category.rejected";

    public const string ServiceAreaVerified = "masters.service-area.verified";

    public const string ServiceAreaRejected = "masters.service-area.rejected";

    public const string VerificationHoldPlaced = "masters.verification-hold.placed";

    public const string VerificationHoldReleased = "masters.verification-hold.released";

    public const string SanctionApplied = "masters.sanction.applied";

    public const string SanctionLifted = "masters.sanction.lifted";

    public const string UploadCompleted = "masters.upload.completed";

    public const string UploadFailed = "masters.upload.failed";
}