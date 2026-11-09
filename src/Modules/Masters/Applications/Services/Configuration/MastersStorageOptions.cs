namespace NoMoreTears.Modules.Masters.Applications.Services.Configuration;

/// <summary>
///     Buckets of the object storage used by the Masters module.
/// </summary>
public sealed class MastersStorageOptions
{
    public const string SectionName = "Storage";

    /// <summary>
    ///     Temporary bucket for received files until the check.
    ///     A lifecycle rule that expires objects after a day is recommended as a safety net.
    /// </summary>
    public string StagingBucket { get; set; } = "masters-staging";

    /// <summary>
    ///     Private bucket of verification documents.
    /// </summary>
    public string DocumentsBucket { get; set; } = "masters-documents";

    /// <summary>
    ///     Bucket of profile photos.
    /// </summary>
    public string AvatarsBucket { get; set; } = "masters-avatars";
}