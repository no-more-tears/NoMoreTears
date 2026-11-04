namespace NoMoreTears.Shared.Domain;

/// <summary>
///     Base class for entities that require auditing information.
/// </summary>
/// <typeparam name="TId">The type of the entity's primary key.</typeparam>
public abstract class AuditableEntity<TId>
    : ISoftDeletable
    where TId : struct
{
    /// <summary>
    ///     Unique identifier of the entity.
    /// </summary>
    public TId Id { get; protected set; }

    /// <summary>
    ///     Timestamp at which the entity was created, or <see langword="null"/> if it has not been set.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; protected set; }

    /// <summary>
    ///     Most recent update to the entity, or <see langword="null"/> if it has never been updated.
    /// </summary>
    public DateTimeOffset? LastUpdatedAt { get; protected set; }

    /// <inheritdoc />
    public bool IsDeleted { get; protected set; }

    /// <inheritdoc />
    public DateTimeOffset? DeletedAt { get; protected set; }

    /// <summary>
    ///     Records the creation timestamp of the entity, if it has not already been set.
    /// </summary>
    /// <param name="createdAt">The timestamp to record as the creation time.</param>
    protected void MarkAsCreated(DateTimeOffset createdAt)
    {
        if (CreatedAt is null)
        {
            CreatedAt = createdAt;
        }
    }

    /// <summary>
    ///     Marks the entity as deleted and records the deletion timestamp, if it is not already deleted.
    /// </summary>
    /// <param name="deletedAt">The timestamp to record as the deletion time.</param>
    protected void MarkAsDeleted(DateTimeOffset deletedAt)
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedAt = deletedAt;
        }
    }

    /// <summary>
    ///     Records the timestamp of the most recent update to the entity.
    /// </summary>
    /// <param name="updatedAt">The timestamp to record as the last update time.</param>
    protected void MarkAsUpdated(DateTimeOffset updatedAt)
    {
        LastUpdatedAt = updatedAt;
    }

    /// <inheritdoc />
    public void Delete(DateTimeOffset deletedAt)
    {
        MarkAsDeleted(deletedAt);
    }

    /// <inheritdoc />
    public void Restore()
    {
        if (IsDeleted)
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }
}