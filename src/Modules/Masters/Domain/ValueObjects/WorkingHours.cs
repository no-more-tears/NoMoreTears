using NoMoreTears.Modules.Masters.Domain.Exceptions;

namespace NoMoreTears.Modules.Masters.Domain.ValueObjects;

/// <summary>
///     Working interval of a master on one day of the week,
///     in the master's local time.
/// </summary>
public sealed record WorkingHours
{
    /// <summary>
    ///     Day of the week.
    /// </summary>
    public DayOfWeek Day { get; }

    /// <summary>
    ///     Start of the interval, inclusive.
    /// </summary>
    public TimeOnly Start { get; }

    /// <summary>
    ///     End of the interval, exclusive.
    /// </summary>
    public TimeOnly End { get; }

    private WorkingHours(
        DayOfWeek day,
        TimeOnly start,
        TimeOnly end)
    {
        Day = day;
        Start = start;
        End = end;
    }

    /// <summary>
    ///     Creates a working interval within one day.
    /// </summary>
    public static WorkingHours Of(
        DayOfWeek day,
        TimeOnly start,
        TimeOnly end)
    {
        if (!Enum.IsDefined(day))
        {
            throw new InvalidWorkScheduleException($"Day '{day}' is not a day of the week.");
        }

        if (start >= end)
        {
            throw new InvalidWorkScheduleException("Working hours must start before they end within the same day.");
        }

        return new WorkingHours(day, start, end);
    }

    /// <summary>
    ///     Returns whether the given local day and time fall within the interval.
    /// </summary>
    public bool Covers(DayOfWeek day, TimeOnly time) => Day == day && time >= Start && time < End;

    /// <summary>
    ///     Returns whether this interval overlaps another one.
    /// </summary>
    public bool Overlaps(WorkingHours other)
    {
        ArgumentNullException.ThrowIfNull(other);

        return Day == other.Day && Start < other.End && other.Start < End;
    }
}