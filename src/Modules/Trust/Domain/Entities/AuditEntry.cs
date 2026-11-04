using NoMoreTears.Modules.Trust.Domain.Enums;
using NoMoreTears.Modules.Trust.Domain.ValueObjects;

namespace NoMoreTears.Modules.Trust.Domain.Entities;

/// <summary>
///     Immutable entry of the audit journal of incidents,
///     decisions, sanctions and temporary measures.
/// </summary>
public sealed class AuditEntry
{
    /// <summary>
    ///     Unique identifier of the entry.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    ///     Recorded action.
    /// </summary>
    public AuditAction Action { get; }

    /// <summary>
    ///     Who performed the action.
    /// </summary>
    public Actor Actor { get; }

    /// <summary>
    ///     Justification, statement or explanation of the action.
    /// </summary>
    public string? Details { get; }

    /// <summary>
    ///     Moment of the action.
    /// </summary>
    public DateTimeOffset OccurredAt { get; }

    private AuditEntry(
        Guid id,
        AuditAction action,
        Actor actor,
        string? details,
        DateTimeOffset occurredAt)
    {
        Id = id;
        Action = action;
        Actor = actor;
        Details = details;
        OccurredAt = occurredAt;
    }

    internal static AuditEntry Create(
        AuditAction action,
        Actor actor,
        string? details,
        DateTimeOffset occurredAt)
    {
        return new AuditEntry(Guid.CreateVersion7(occurredAt), action, actor, details, occurredAt);
    }
}