namespace NoMoreTears.Modules.Requests.Applications.Services.Configuration;

/// <summary>
///     Buckets of the object storage used by the Requests module.
/// </summary>
public sealed class RequestsStorageOptions
{
    public const string SectionName = "Storage";

    /// <summary>
    ///     Temporary bucket for received photos until the check.
    /// </summary>
    public string StagingBucket { get; set; } = "requests-staging";

    /// <summary>
    ///     Private bucket of photos attached to requests.
    /// </summary>
    public string MediaBucket { get; set; } = "requests-media";
}