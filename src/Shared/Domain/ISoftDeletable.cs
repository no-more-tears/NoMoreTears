namespace NoMoreTears.Shared.Domain;

/// <summary>
///     Contract for entities that support soft deletion.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    ///     Value indicating whether the entity has been soft-deleted.
    /// </summary>
    bool IsDeleted { get; }

    /// <summary>
    ///     Timestamp at which the entity was soft-deleted, or <see langword="null"/> if it has not been deleted.
    /// </summary>
    DateTimeOffset? DeletedAt { get; }

    /// <summary>
    ///     Marks the entity as deleted without physically removing it from the underlying store.
    /// </summary>
    /// <param name="deletedAt">The timestamp to record as the deletion time.</param>
    void Delete(DateTimeOffset deletedAt);

    /// <summary>
    ///     Reverts a previous soft deletion, restoring the entity to an active state.
    /// </summary>
    void Restore();
}