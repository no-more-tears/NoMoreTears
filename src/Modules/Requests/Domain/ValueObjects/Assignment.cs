using NoMoreTears.Modules.Requests.Domain.Exceptions;

namespace NoMoreTears.Modules.Requests.Domain.ValueObjects;

/// <summary>
///     Master who accepted the request together with the expected arrival time.
/// </summary>
public sealed record Assignment
{
    /// <summary>
    ///     Identifier of the master.
    /// </summary>
    public Guid MasterId { get; }

    /// <summary>
    ///     Moment of acceptance.
    /// </summary>
    public DateTimeOffset AcceptedAt { get; }

    /// <summary>
    ///     Moment by which the master is expected to arrive.
    /// </summary>
    public DateTimeOffset ExpectedArrivalAt { get; }

    /// <summary>
    ///     Moment from which the client can report a no-show.
    /// </summary>
    public DateTimeOffset NoShowReportableFrom => ExpectedArrivalAt + RequestRules.NoShowBuffer;

    private Assignment(
        Guid masterId,
        DateTimeOffset acceptedAt,
        DateTimeOffset expectedArrivalAt)
    {
        MasterId = masterId;
        AcceptedAt = acceptedAt;
        ExpectedArrivalAt = expectedArrivalAt;
    }

    /// <summary>
    ///     Creates an assignment of a master who accepted the request.
    /// </summary>
    public static Assignment Of(
        Guid masterId,
        DateTimeOffset acceptedAt,
        TimeSpan arrivalInterval)
    {
        if (masterId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(masterId));
        }

        if (arrivalInterval <= TimeSpan.Zero)
        {
            throw new InvalidAssignmentException("Arrival interval must be positive.");
        }

        return new Assignment(masterId, acceptedAt, acceptedAt + arrivalInterval);
    }

    /// <summary>
    ///     Returns whether the given master is the assigned one.
    /// </summary>
    public bool IsAssignedTo(Guid masterId) => MasterId == masterId;

    /// <summary>
    ///     Returns whether the expected arrival time plus the buffer has passed at the given moment.
    /// </summary>
    public bool IsArrivalOverdueAt(DateTimeOffset moment) => moment >= NoShowReportableFrom;
}