using NoMoreTears.Modules.Masters.Domain.Enums;
using NoMoreTears.Modules.Masters.Domain.Exceptions;
using NoMoreTears.Modules.Masters.Domain.ValueObjects;
using NoMoreTears.Shared.Domain;

namespace NoMoreTears.Modules.Masters.Domain.Aggregates;

/// <summary>
///     Service category of the catalog maintained by PlatformAdministrator.
///     The catalog is the single source of truth for Requests and Dispatch.
/// </summary>
public sealed class Category : AuditableEntity<Guid>
{
    /// <summary>
    ///     Name of the category.
    /// </summary>
    public CategoryName Name { get; private set; }

    /// <summary>
    ///     Optional description of the category.
    /// </summary>
    public CategoryDescription? Description { get; private set; }

    /// <summary>
    ///     Status of the category in the catalog.
    /// </summary>
    public CategoryStatus Status { get; private set; }

    /// <summary>
    ///     Whether masters and clients can choose the category.
    /// </summary>
    public bool IsActive => Status is CategoryStatus.Active && !IsDeleted;

    /// <summary>
    ///     Used only by persistence to materialize the category.
    /// </summary>
    private Category()
    {
        Name = null!;
    }

    private Category(
        Guid id,
        CategoryName name,
        CategoryDescription? description)
    {
        Id = id;
        Name = name;
        Description = description;
        Status = CategoryStatus.Active;
    }

    /// <summary>
    ///     Creates an active category.
    /// </summary>
    public static Category Create(
        CategoryName name,
        CategoryDescription? description,
        DateTimeOffset createdAt)
    {
        ArgumentNullException.ThrowIfNull(name);

        var category = new Category(Guid.CreateVersion7(createdAt), name, description);
        category.MarkAsCreated(createdAt);

        return category;
    }

    /// <summary>
    ///     Renames the category.
    /// </summary>
    public void Rename(
        CategoryName name,
        DateTimeOffset renamedAt)
    {
        ArgumentNullException.ThrowIfNull(name);

        Name = name;
        MarkAsUpdated(renamedAt);
    }

    /// <summary>
    ///     Replaces or removes the description.
    /// </summary>
    public void ChangeDescription(
        CategoryDescription? description,
        DateTimeOffset changedAt)
    {
        Description = description;
        MarkAsUpdated(changedAt);
    }

    /// <summary>
    ///     Withdraws the category from choice for new requests and master profiles.
    /// </summary>
    public void Archive(DateTimeOffset archivedAt)
    {
        if (Status is not CategoryStatus.Active)
        {
            throw new InvalidCategoryStatusTransitionException(Status, "archive");
        }

        Status = CategoryStatus.Archived;
        MarkAsUpdated(archivedAt);
    }

    /// <summary>
    ///     Returns an archived category to choice.
    /// </summary>
    public void Activate(DateTimeOffset activatedAt)
    {
        if (Status is not CategoryStatus.Archived)
        {
            throw new InvalidCategoryStatusTransitionException(Status, "activate");
        }

        Status = CategoryStatus.Active;
        MarkAsUpdated(activatedAt);
    }
}