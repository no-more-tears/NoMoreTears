using NoMoreTears.Modules.Dispatch.Domain.Exceptions;

namespace NoMoreTears.Modules.Dispatch.Domain.ValueObjects;

/// <summary>
///     Settings of one wave.
///     The final wave covers all candidates and lasts until the acceptance deadline.
/// </summary>
public sealed record WaveSettings
{
    /// <summary>
    ///     Default plan: top 20 % but at least 5 for 10 minutes, top 50 % for 20 minutes, then everyone.
    /// </summary>
    public static IReadOnlyList<WaveSettings> DefaultPlan { get; } =
    [
        new(1, 0.20, 5, TimeSpan.FromMinutes(10)),
        new(2, 0.50, 0, TimeSpan.FromMinutes(20)),
        new(3, 1.00, 0, null)
    ];

    /// <summary>
    ///     Number of the wave, starting from 1.
    /// </summary>
    public int Number { get; }

    /// <summary>
    ///     Cumulative share of the best candidates offered up to and including this wave.
    /// </summary>
    public double TopShare { get; }

    /// <summary>
    ///     Cumulative minimum number of candidates offered up to and including this wave.
    /// </summary>
    public int MinimumCandidates { get; }

    /// <summary>
    ///     Duration of the wave.
    /// </summary>
    public TimeSpan? Window { get; }

    /// <summary>
    ///     Whether this is the final wave, which covers all candidates until the deadline.
    /// </summary>
    public bool IsFinal => Window is null;

    private WaveSettings(
        int number,
        double topShare,
        int minimumCandidates,
        TimeSpan? window)
    {
        Number = number;
        TopShare = topShare;
        MinimumCandidates = minimumCandidates;
        Window = window;
    }

    /// <summary>
    ///     Creates settings of an intermediate wave.
    /// </summary>
    public static WaveSettings Of(
        int number,
        double topShare,
        int minimumCandidates,
        TimeSpan window)
    {
        if (window <= TimeSpan.Zero)
        {
            throw new InvalidPolicyException("Wave window must be positive.");
        }

        if (!double.IsFinite(topShare) || topShare <= 0 || topShare >= 1)
        {
            throw new InvalidPolicyException("Top share of an intermediate wave must be greater than 0 and less than 1.");
        }

        EnsureNumberAndMinimum(number, minimumCandidates);
        return new WaveSettings(number, topShare, minimumCandidates, window);
    }

    /// <summary>
    ///     Creates settings of the final wave.
    /// </summary>
    public static WaveSettings Of(int number)
    {
        EnsureNumberAndMinimum(number, 0);

        return new WaveSettings(number, 1, 0, null);
    }

    /// <summary>
    ///     Calculates how many of the best candidates are offered up to and including this wave.
    /// </summary>
    public int CumulativeSizeFor(int candidatesCount)
    {
        if (IsFinal)
        {
            return candidatesCount;
        }

        var byShare = (int)Math.Ceiling((candidatesCount * TopShare) - DispatchRules.Tolerance);

        return Math.Min(candidatesCount, Math.Max(MinimumCandidates, byShare));
    }

    private static void EnsureNumberAndMinimum(int number, int minimumCandidates)
    {
        if (number < 1)
        {
            throw new InvalidPolicyException("Wave number must start from 1.");
        }

        if (minimumCandidates < 0)
        {
            throw new InvalidPolicyException("Minimum number of candidates must not be negative.");
        }
    }
}