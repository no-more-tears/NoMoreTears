namespace NoMoreTears.Modules.Requests.HostSide.BackgroundJobs.Configuration;

/// <summary>
///     Schedule of the background jobs of the Requests module.
/// </summary>
public sealed class RequestJobsOptions
{
    public const string SectionName = "Requests:Jobs";

    /// <summary>
    ///     How often published requests are checked for a passed acceptance deadline.
    /// </summary>
    public TimeSpan ExpirationInterval { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    ///     How often requests marked as done are checked for a passed confirmation period.
    /// </summary>
    public TimeSpan CompletionInterval { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    ///     Requests processed in one transaction; a run repeats batches until fewer are due.
    /// </summary>
    public int BatchSize { get; set; } = 100;
}