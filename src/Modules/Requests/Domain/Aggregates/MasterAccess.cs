using NoMoreTears.Modules.Requests.Domain.Exceptions;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Aggregates;

/// <summary>
///     Local snapshot of the access of a master published by Masters: which categories the master may accept.
///     The identifier is the master id. Only a verified master with a verified service area has categories here.
/// </summary>
public sealed class MasterAccess : AuditableEntity<Guid>
{
    private List<Guid> _acceptableCategoryIds = [];

    /// <summary>
    ///     Verified categories of a verified master; empty when the master has no access to the feed.
    /// </summary>
    public IReadOnlyCollection<Guid> AcceptableCategoryIds => _acceptableCategoryIds.AsReadOnly();

    /// <summary>
    ///     End of an active suspension; <see cref="DateTimeOffset.MaxValue"/> means until it is lifted.
    /// </summary>
    public DateTimeOffset? SuspendedUntil { get; private set; }

    /// <summary>
    ///     Moment of the Masters state the snapshot reflects; older states are ignored.
    /// </summary>
    public DateTimeOffset SnapshotAt { get; private set; }

    private MasterAccess(
        Guid id,
        DateTimeOffset snapshotAt)
    {
        Id = id;
        SnapshotAt = snapshotAt;
    }

    /// <summary>
    ///     Creates the snapshot of a master.
    /// </summary>
    public static MasterAccess Create(
        Guid masterId,
        IReadOnlyCollection<Guid> acceptableCategoryIds,
        DateTimeOffset? suspendedUntil,
        DateTimeOffset snapshotAt)
    {
        ArgumentNullException.ThrowIfNull(acceptableCategoryIds);

        if (masterId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(masterId));
        }

        var access = new MasterAccess(masterId, snapshotAt)
        {
            _acceptableCategoryIds = [.. acceptableCategoryIds.Distinct()],
            SuspendedUntil = suspendedUntil
        };

        access.MarkAsCreated(snapshotAt);
        return access;
    }

    /// <summary>
    ///     Replaces the snapshot with a newer state; returns <c>false</c> when the given state is older.
    /// </summary>
    public bool Refresh(
        IReadOnlyCollection<Guid> acceptableCategoryIds,
        DateTimeOffset? suspendedUntil,
        DateTimeOffset snapshotAt)
    {
        ArgumentNullException.ThrowIfNull(acceptableCategoryIds);

        if (snapshotAt < SnapshotAt)
        {
            return false;
        }

        _acceptableCategoryIds = [.. acceptableCategoryIds.Distinct()];
        SuspendedUntil = suspendedUntil;
        SnapshotAt = snapshotAt;
        MarkAsUpdated(snapshotAt);

        return true;
    }

    /// <summary>
    ///     Returns whether the master may accept a request of the category at the given moment.
    ///     A suspension that ended by time no longer applies.
    /// </summary>
    public bool CanAcceptAt(
        Guid categoryId,
        DateTimeOffset moment)
    {
        return _acceptableCategoryIds.Contains(categoryId)
            && (SuspendedUntil is null || moment >= SuspendedUntil);
    }

    /// <summary>
    ///     Ensures that the master may accept a request of the category at the given moment.
    /// </summary>
    public void EnsureCanAccept(
        Guid categoryId,
        DateTimeOffset moment)
    {
        if (!CanAcceptAt(categoryId, moment))
        {
            throw new MasterNotEligibleException();
        }
    }
}