using NoMoreTears.Modules.Dispatch.Domain.Exceptions;

namespace NoMoreTears.Modules.Dispatch.Domain.ValueObjects;

/// <summary>
///     Limits that protect the flow from accumulation and automation.
/// </summary>
public sealed record MasterLimits
{
    /// <summary>
    ///     Default limits: 3 active requests, 30 minutes of cooldown, 10 attempts per minute,
    ///     1 acceptance per 10 minutes for a throttled master.
    /// </summary>
    public static MasterLimits Default { get; } = new(3, TimeSpan.FromMinutes(30), 10, TimeSpan.FromMinutes(10));

    /// <summary>
    ///     Maximum number of active requests of a master.
    /// </summary>
    public int ActiveRequestsLimit { get; }

    /// <summary>
    ///     Cooldown after a refusal initiated by the master.
    /// </summary>
    public TimeSpan CooldownAfterRefusal { get; }

    /// <summary>
    ///     Maximum number of acceptance attempts per minute.
    /// </summary>
    public int AcceptanceAttemptsPerMinute { get; }

    /// <summary>
    ///     Minimum interval between acceptances of a throttled master.
    /// </summary>
    public TimeSpan ThrottledAcceptanceInterval { get; }

    private MasterLimits(
        int activeRequestsLimit,
        TimeSpan cooldownAfterRefusal,
        int acceptanceAttemptsPerMinute,
        TimeSpan throttledAcceptanceInterval)
    {
        ActiveRequestsLimit = activeRequestsLimit;
        CooldownAfterRefusal = cooldownAfterRefusal;
        AcceptanceAttemptsPerMinute = acceptanceAttemptsPerMinute;
        ThrottledAcceptanceInterval = throttledAcceptanceInterval;
    }

    /// <summary>
    ///     Creates master limits.
    /// </summary>
    public static MasterLimits Of(
        int activeRequestsLimit,
        TimeSpan cooldownAfterRefusal,
        int acceptanceAttemptsPerMinute,
        TimeSpan throttledAcceptanceInterval)
    {
        if (activeRequestsLimit < 1 || acceptanceAttemptsPerMinute < 1)
        {
            throw new InvalidPolicyException("Active requests limit and acceptance attempts limit must be at least 1.");
        }

        if (cooldownAfterRefusal < TimeSpan.Zero || throttledAcceptanceInterval <= TimeSpan.Zero)
        {
            throw new InvalidPolicyException("Cooldown must not be negative and throttled interval must be positive.");
        }

        return new MasterLimits(
            activeRequestsLimit,
            cooldownAfterRefusal,
            acceptanceAttemptsPerMinute,
            throttledAcceptanceInterval);
    }
}