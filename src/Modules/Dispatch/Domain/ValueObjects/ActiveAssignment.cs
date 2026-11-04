namespace NoMoreTears.Modules.Dispatch.Domain.ValueObjects;

/// <summary>
///     Request accepted by a master and not finished yet;
///     counts toward the active requests limit.
/// </summary>
public sealed record ActiveAssignment
{
    /// <summary>
    ///     Identifier of the request.
    /// </summary>
    public Guid RequestId { get; }

    /// <summary>
    ///     Moment of acceptance.
    /// </summary>
    public DateTimeOffset AcceptedAt { get; }

    private ActiveAssignment(
        Guid requestId,
        DateTimeOffset acceptedAt)
    {
        RequestId = requestId;
        AcceptedAt = acceptedAt;
    }

    /// <summary>
    ///     Creates an active assignment.
    /// </summary>
    public static ActiveAssignment Of(
        Guid requestId,
        DateTimeOffset acceptedAt)
    {
        return new ActiveAssignment(requestId, acceptedAt);
    }
}