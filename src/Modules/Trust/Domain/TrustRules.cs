using System.Collections.Frozen;

namespace NoMoreTears.Modules.Trust.Domain;

/// <summary>
///     Limits and periods of the Trust bounded context.
/// </summary>
public static class TrustRules
{
    /// <summary>
    ///     Minimum rating.
    /// </summary>
    public const int MinRating = 1;

    /// <summary>
    ///     Maximum rating.
    /// </summary>
    public const int MaxRating = 5;

    /// <summary>
    ///     Platform prior k of the Bayesian reputation average.
    /// </summary>
    public const int ReputationPrior = 10;

    /// <summary>
    ///     Age in days after which the weight of a review halves.
    /// </summary>
    public const double ReviewHalfLifeInDays = 180;

    /// <summary>
    ///     Weight of reviews of a repeated client–master pair.
    /// </summary>
    public const double RepeatedPairReviewWeight = 0.5;

    /// <summary>
    ///     Maximum length of a review comment.
    /// </summary>
    public const int MaxReviewCommentLength = 2000;

    /// <summary>
    ///     Maximum length of a statement of a participant: complaint, evidence, appeal or contest.
    /// </summary>
    public const int MaxStatementLength = 4000;

    /// <summary>
    ///     Maximum length of a justification of a decision.
    /// </summary>
    public const int MaxJustificationLength = 2000;

    /// <summary>
    ///     Maximum length of an explanation of a temporary measure shown to the user.
    /// </summary>
    public const int MaxExplanationLength = 1000;

    /// <summary>
    ///     Maximum number of files in one piece of evidence.
    /// </summary>
    public const int MaxEvidenceFiles = 10;

    /// <summary>
    ///     Maximum size of one evidence file: 25 MB.
    /// </summary>
    public const long MaxEvidenceFileSizeInBytes = 25L * 1024 * 1024;

    /// <summary>
    ///     Maximum length of the object storage key of an evidence file.
    /// </summary>
    public const int MaxStorageKeyLength = 1024;

    /// <summary>
    ///     Period after completion during which the client can leave a review.
    /// </summary>
    public static readonly TimeSpan ReviewWindow = TimeSpan.FromDays(14);

    /// <summary>
    ///     Period after completion or failure during which a participant can complain.
    /// </summary>
    public static readonly TimeSpan ComplaintWindow = TimeSpan.FromDays(14);

    /// <summary>
    ///     Period of evidence collection from both parties.
    /// </summary>
    public static readonly TimeSpan EvidenceCollectionPeriod = TimeSpan.FromHours(48);

    /// <summary>
    ///     SLA of the first reaction of an arbiter.
    /// </summary>
    public static readonly TimeSpan FirstReactionPeriod = TimeSpan.FromHours(48);

    /// <summary>
    ///     SLA of the arbitration decision.
    /// </summary>
    public static readonly TimeSpan DecisionPeriod = TimeSpan.FromDays(7);

    /// <summary>
    ///     Period after a decision during which a party can appeal.
    /// </summary>
    public static readonly TimeSpan AppealWindow = TimeSpan.FromDays(7);

    /// <summary>
    ///     Period within which a human must review a temporary measure before it is escalated to PlatformAdministrator.
    /// </summary>
    public static readonly TimeSpan HumanReviewPeriod = TimeSpan.FromHours(48);

    /// <summary>
    ///     Duration of a restriction (ladder step 2).
    /// </summary>
    public static readonly TimeSpan RestrictionDuration = TimeSpan.FromDays(7);

    /// <summary>
    ///     Duration of a suspension or publication block (ladder step 3).
    /// </summary>
    public static readonly TimeSpan SuspensionDuration = TimeSpan.FromDays(30);

    /// <summary>
    ///     Content types accepted for evidence files.
    /// </summary>
    public static FrozenSet<string> AllowedEvidenceContentTypes { get; } = new[]
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/heic",
        "video/mp4",
        "application/pdf"
    }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
}