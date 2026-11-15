using NoMoreTears.Modules.Requests.Domain.Exceptions;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Requests.Domain.Aggregates;

/// <summary>
///     Local snapshot of a category of the Masters catalog, the single source of truth for categories.
/// </summary>
public sealed class ServiceCategory : AuditableEntity<Guid>
{
    /// <summary>
    ///     Name of the category.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Whether the category can be chosen for new requests.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    ///     Moment of the Masters state the snapshot reflects; older states are ignored.
    /// </summary>
    public DateTimeOffset SnapshotAt { get; private set; }

    private ServiceCategory(
        Guid id,
        string name,
        DateTimeOffset snapshotAt)
    {
        Id = id;
        Name = name;
        SnapshotAt = snapshotAt;
        IsActive = true;
    }

    /// <summary>
    ///     Creates the snapshot of an active category.
    /// </summary>
    public static ServiceCategory Create(
        Guid categoryId,
        string name,
        DateTimeOffset snapshotAt)
    {
        if (categoryId == Guid.Empty)
        {
            throw new InvalidIdentifierException(nameof(categoryId));
        }

        var category = new ServiceCategory(categoryId, Normalize(name), snapshotAt);
        category.MarkAsCreated(snapshotAt);

        return category;
    }

    /// <summary>
    ///     Applies a new name; returns <c>false</c> when the change is older than the snapshot.
    /// </summary>
    public bool Rename(
        string name,
        DateTimeOffset changedAt)
    {
        if (changedAt < SnapshotAt)
        {
            return false;
        }

        Name = Normalize(name);
        Touch(changedAt);

        return true;
    }

    /// <summary>
    ///     Archives the category; returns <c>false</c> when the change is older than the snapshot.
    /// </summary>
    public bool Archive(DateTimeOffset archivedAt)
    {
        if (archivedAt < SnapshotAt)
        {
            return false;
        }

        IsActive = false;
        Touch(archivedAt);

        return true;
    }

    /// <summary>
    ///     Activates the category again; returns <c>false</c> when the change is older than the snapshot.
    /// </summary>
    public bool Activate(DateTimeOffset activatedAt)
    {
        if (activatedAt < SnapshotAt)
        {
            return false;
        }

        IsActive = true;
        Touch(activatedAt);

        return true;
    }

    /// <summary>
    ///     Ensures that the category can be chosen for a request.
    /// </summary>
    public void EnsureCanBeChosen()
    {
        if (!IsActive)
        {
            throw new CategoryUnavailableException(Id);
        }
    }

    private void Touch(DateTimeOffset changedAt)
    {
        SnapshotAt = changedAt;
        MarkAsUpdated(changedAt);
    }

    private static string Normalize(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var normalized = name.Trim();
        return normalized.Length > RequestRules.MaxCategoryNameLength
            ? normalized[..RequestRules.MaxCategoryNameLength]
            : normalized;
    }
}