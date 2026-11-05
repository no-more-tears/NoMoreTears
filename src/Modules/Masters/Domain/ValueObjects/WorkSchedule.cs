using NoMoreTears.Modules.Masters.Domain.Exceptions;

namespace NoMoreTears.Modules.Masters.Domain.ValueObjects;

/// <summary>
///     Weekly work schedule of a master in the master's time zone.
/// </summary>
public sealed record WorkSchedule
{
    /// <summary>
    ///     IANA identifier of the master's time zone, for example <c>Europe/Kyiv</c>.
    /// </summary>
    public string TimeZoneId { get; }

    /// <summary>
    ///     Working intervals ordered by day and start time.
    /// </summary>
    public IReadOnlyList<WorkingHours> Hours { get; }

    private WorkSchedule(
        string timeZoneId,
        IReadOnlyList<WorkingHours> hours)
    {
        TimeZoneId = timeZoneId;
        Hours = hours;
    }

    /// <summary>
    ///     Creates a work schedule.
    /// </summary>
    public static WorkSchedule Of(
        string timeZoneId,
        IReadOnlyCollection<WorkingHours> hours)
    {
        ArgumentNullException.ThrowIfNull(hours);

        var normalizedTimeZoneId = timeZoneId?.Trim() ?? string.Empty;

        if (normalizedTimeZoneId.Length == 0 || !TimeZoneInfo.TryFindSystemTimeZoneById(normalizedTimeZoneId, out _))
        {
            throw new InvalidWorkScheduleException($"Time zone '{normalizedTimeZoneId}' is unknown.");
        }

        if (hours.Count == 0)
        {
            throw new InvalidWorkScheduleException("Work schedule must contain at least one working interval.");
        }

        var ordered = hours.OrderBy(interval => interval.Day).ThenBy(interval => interval.Start).ToArray();

        for (var index = 1; index < ordered.Length; index++)
        {
            if (ordered[index - 1].Overlaps(ordered[index]))
            {
                throw new InvalidWorkScheduleException($"Working intervals on {ordered[index].Day} overlap.");
            }
        }

        return new WorkSchedule(normalizedTimeZoneId, ordered);
    }

    /// <summary>
    ///     Returns whether the master works at the given moment.
    /// </summary>
    public bool IsWorkingAt(DateTimeOffset moment)
    {
        var local = TimeZoneInfo.ConvertTime(moment, TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId));
        var time = TimeOnly.FromTimeSpan(local.TimeOfDay);

        return Hours.Any(interval => interval.Covers(local.DayOfWeek, time));
    }

    /// <inheritdoc />
    public bool Equals(WorkSchedule? other)
    {
        return other is not null
            && string.Equals(TimeZoneId, other.TimeZoneId, StringComparison.Ordinal)
            && Hours.SequenceEqual(other.Hours);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(TimeZoneId, StringComparer.Ordinal);

        foreach (var interval in Hours)
        {
            hash.Add(interval);
        }

        return hash.ToHashCode();
    }
}