namespace NoMoreTears.Modules.Trust.Domain;

/// <summary>
///     Thresholds of fraud signals. Services count the events, these rules decide.
/// </summary>
public static class SignalRules
{
    /// <summary>
    ///     Number of 5-star reviews within 24 hours from accounts younger than 7 days that makes a rating burst.
    /// </summary>
    public const int RatingBurstThreshold = 5;

    /// <summary>
    ///     Number of completed 5-star requests of one client–master pair within 30 days that makes a repeated pair.
    /// </summary>
    public const int RepeatedPairThreshold = 3;

    /// <summary>
    ///     Number of confirmed no-shows within 30 days that triggers the repeated no-shows rule.
    /// </summary>
    public const int RepeatedNoShowsThreshold = 2;

    /// <summary>
    ///     Number of cancellations by a master within 7 days that triggers the cancellation series rule.
    /// </summary>
    public const int MasterCancellationSeriesThreshold = 3;

    /// <summary>
    ///     Number of cancellations by a client after acceptance within 30 days that triggers the client abuse rule.
    /// </summary>
    public const int ClientAbuseThreshold = 3;

    /// <summary>
    ///     Number of rejected complaints within 90 days after which complaints require media evidence.
    /// </summary>
    public const int FalseComplaintsThreshold = 3;

    /// <summary>
    ///     Age of an account below which its reviews count toward a rating burst.
    /// </summary>
    public static readonly TimeSpan YoungAccountAge = TimeSpan.FromDays(7);

    /// <summary>
    ///     Returns whether the 5-star reviews from young accounts within 24 hours make a rating burst.
    /// </summary>
    public static bool IsRatingBurst(int fiveStarReviewsFromYoungAccountsLastDay)
    {
        return fiveStarReviewsFromYoungAccountsLastDay >= RatingBurstThreshold;
    }

    /// <summary>
    ///     Returns whether the client–master pair is a repeated pair whose reviews lose weight.
    /// </summary>
    public static bool IsRepeatedPair(int fiveStarCompletedRequestsOfPairLast30Days)
    {
        return fiveStarCompletedRequestsOfPairLast30Days >= RepeatedPairThreshold;
    }

    /// <summary>
    ///     Returns whether the confirmed no-shows of a master trigger the repeated no-shows rule.
    /// </summary>
    public static bool IsRepeatedNoShows(int confirmedNoShowsLast30Days)
    {
        return confirmedNoShowsLast30Days >= RepeatedNoShowsThreshold;
    }

    /// <summary>
    ///     Returns whether the cancellations of a master trigger the cancellation series rule.
    /// </summary>
    public static bool IsMasterCancellationSeries(int masterCancellationsLast7Days)
    {
        return masterCancellationsLast7Days >= MasterCancellationSeriesThreshold;
    }

    /// <summary>
    ///     Returns whether the cancellations of a client after acceptance trigger the client abuse rule.
    /// </summary>
    public static bool IsClientAbuse(int cancellationsAfterAcceptanceLast30Days)
    {
        return cancellationsAfterAcceptanceLast30Days >= ClientAbuseThreshold;
    }

    /// <summary>
    ///     Returns whether complaints of a participant must contain media evidence.
    /// </summary>
    public static bool RequiresMediaEvidence(int rejectedComplaintsLast90Days)
    {
        return rejectedComplaintsLast90Days >= FalseComplaintsThreshold;
    }
}