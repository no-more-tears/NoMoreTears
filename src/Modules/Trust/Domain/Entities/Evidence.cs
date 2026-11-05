using NoMoreTears.Modules.Trust.Domain.Exceptions;
using NoMoreTears.Modules.Trust.Domain.ValueObjects;

namespace NoMoreTears.Modules.Trust.Domain.Entities;

/// <summary>
///     Evidence submitted by a party of an incident: a statement with optional media files.
/// </summary>
public sealed class Evidence
{
    /// <summary>
    ///     Unique identifier of the evidence.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    ///     Party who submitted the evidence.
    /// </summary>
    public Party SubmittedBy { get; }

    /// <summary>
    ///     Statement of the party.
    /// </summary>
    public Statement Statement { get; }

    /// <summary>
    ///     Media files.
    /// </summary>
    public IReadOnlyList<EvidenceFile> Files { get; }

    /// <summary>
    ///     Moment of submission.
    /// </summary>
    public DateTimeOffset SubmittedAt { get; }

    /// <summary>
    ///     Whether the evidence contains media files.
    /// </summary>
    public bool HasMedia => Files.Count > 0;

    private Evidence(
        Guid id,
        Party submittedBy,
        Statement statement,
        IReadOnlyList<EvidenceFile> files,
        DateTimeOffset submittedAt)
    {
        Id = id;
        SubmittedBy = submittedBy;
        Statement = statement;
        Files = files;
        SubmittedAt = submittedAt;
    }

    /// <summary>
    ///     Creates evidence. Only the <see cref="Aggregates.Incident"/> aggregate collects evidence.
    /// </summary>
    internal static Evidence Create(
        Party submittedBy,
        Statement statement,
        IReadOnlyCollection<EvidenceFile> files,
        DateTimeOffset submittedAt)
    {
        ArgumentNullException.ThrowIfNull(submittedBy);
        ArgumentNullException.ThrowIfNull(statement);
        ArgumentNullException.ThrowIfNull(files);

        if (files.Count > TrustRules.MaxEvidenceFiles)
        {
            throw new EvidenceFileLimitExceededException(TrustRules.MaxEvidenceFiles);
        }

        return new Evidence(Guid.CreateVersion7(submittedAt), submittedBy, statement, [.. files], submittedAt);
    }
}