using NoMoreTears.Modules.Dispatch.Domain.Exceptions;

namespace NoMoreTears.Modules.Dispatch.Domain.ValueObjects;

/// <summary>
///     Share of the slots of every wave guaranteed to newcomers.
/// </summary>
public sealed record NewcomerQuota
{
    /// <summary>
    ///     Default quota: 15 % of slots for masters with fewer than 10 completed requests.
    /// </summary>
    public static NewcomerQuota Default { get; } = new(0.15, 10);

    /// <summary>
    ///     Share of the slots of every wave.
    /// </summary>
    public double Share { get; }

    /// <summary>
    ///     A master with fewer completed requests than this is a newcomer.
    /// </summary>
    public int CompletedRequestsThreshold { get; }

    private NewcomerQuota(
        double share,
        int completedRequestsThreshold)
    {
        Share = share;
        CompletedRequestsThreshold = completedRequestsThreshold;
    }

    /// <summary>
    ///     Creates a newcomer quota.
    /// </summary>
    public static NewcomerQuota Of(
        double share,
        int completedRequestsThreshold)
    {
        if (!double.IsFinite(share) || share < 0 || share > 1)
        {
            throw new InvalidPolicyException("Newcomer share must be from 0 to 1.");
        }

        if (completedRequestsThreshold < 1)
        {
            throw new InvalidPolicyException("Newcomer threshold must be at least 1 completed request.");
        }

        return new NewcomerQuota(share, completedRequestsThreshold);
    }

    /// <summary>
    ///     Returns whether a master with the given number of completed requests is a newcomer.
    /// </summary>
    public bool IsNewcomer(int completedRequests) => completedRequests < CompletedRequestsThreshold;

    /// <summary>
    ///     Calculates how many of the given slots are reserved for newcomers.
    /// </summary>
    public int ReservedSlotsOf(int slots) => (int)Math.Ceiling((slots * Share) - DispatchRules.Tolerance);
}