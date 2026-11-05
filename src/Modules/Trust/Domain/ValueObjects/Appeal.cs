namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Appeal of a party against an arbitration decision.
/// </summary>
public sealed record Appeal
{
    /// <summary>
    ///     Party who appealed.
    /// </summary>
    public Party Appellant { get; }

    /// <summary>
    ///     Grounds of the appeal.
    /// </summary>
    public Statement Statement { get; }

    /// <summary>
    ///     Moment of submission.
    /// </summary>
    public DateTimeOffset SubmittedAt { get; }

    private Appeal(
        Party appellant,
        Statement statement,
        DateTimeOffset submittedAt)
    {
        Appellant = appellant;
        Statement = statement;
        SubmittedAt = submittedAt;
    }

    /// <summary>
    ///     Creates an appeal.
    /// </summary>
    public static Appeal Of(
        Party appellant,
        Statement statement,
        DateTimeOffset submittedAt)
    {
        ArgumentNullException.ThrowIfNull(appellant);
        ArgumentNullException.ThrowIfNull(statement);

        return new Appeal(appellant, statement, submittedAt);
    }
}