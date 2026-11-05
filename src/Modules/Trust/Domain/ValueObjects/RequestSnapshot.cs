using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Modules.Trust.Domain.Exceptions;

namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Request as published by Requests.
/// </summary>
public sealed record RequestSnapshot
{
    /// <summary>
    ///     Identifier of the request.
    /// </summary>
    public Guid RequestId { get; }

    /// <summary>
    ///     Client of the request.
    /// </summary>
    public Guid ClientId { get; }

    /// <summary>
    ///     Master who accepted the request, if any.
    /// </summary>
    public Guid? MasterId { get; }

    /// <summary>
    ///     State of the request.
    /// </summary>
    public RequestState State { get; }

    /// <summary>
    ///     Moment of completion or failure.
    /// </summary>
    public DateTimeOffset? FinishedAt { get; }

    private RequestSnapshot(
        Guid requestId,
        Guid clientId,
        Guid? masterId,
        RequestState state,
        DateTimeOffset? finishedAt)
    {
        RequestId = requestId;
        ClientId = clientId;
        MasterId = masterId;
        State = state;
        FinishedAt = finishedAt;
    }

    /// <summary>
    ///     Creates a request snapshot.
    /// </summary>
    public static RequestSnapshot Of(
        Guid requestId,
        Guid clientId,
        Guid? masterId,
        RequestState state,
        DateTimeOffset? finishedAt)
    {
        if (requestId == Guid.Empty || clientId == Guid.Empty || masterId == Guid.Empty)
        {
            throw new InvalidRequestSnapshotException("Request, client and master identifiers must not be empty.");
        }

        if (state is (RequestState.InProgress or RequestState.Completed or RequestState.Failed) && masterId is null)
        {
            throw new InvalidRequestSnapshotException("An accepted request must have a master.");
        }

        if ((state is RequestState.Completed or RequestState.Failed) != finishedAt.HasValue)
        {
            throw new InvalidRequestSnapshotException("Only a completed or failed request has a finish moment.");
        }

        if (masterId == clientId)
        {
            throw new InvalidRequestSnapshotException("The client and the master must be different users.");
        }

        return new RequestSnapshot(requestId, clientId, masterId, state, finishedAt);
    }

    /// <summary>
    ///     Returns whether the party is the client or the master of the request.
    /// </summary>
    public bool Involves(Party party)
    {
        ArgumentNullException.ThrowIfNull(party);

        return party.Role is PartyRole.Client
            ? party.UserId == ClientId
            : party.UserId == MasterId;
    }

    /// <summary>
    ///     Returns the other participant of the request.
    /// </summary>
    public Party CounterpartyOf(Party party)
    {
        if (!Involves(party) || MasterId is null)
        {
            throw new PartyMismatchException("The party is not a participant of the request.");
        }

        return party.Role is PartyRole.Client
            ? Party.Of(PartyRole.Master, MasterId.Value)
            : Party.Of(PartyRole.Client, ClientId);
    }
}