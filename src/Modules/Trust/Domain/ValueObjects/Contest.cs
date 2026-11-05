namespace NoMoreTears.Modules.Trust.Domain.ValueObjects;

/// <summary>
///     Objection of the subject against a temporary measure,
///     which sends it to human review.
/// </summary>
public sealed record Contest
{
    /// <summary>
    ///     Grounds of the objection.
    /// </summary>
    public Statement Statement { get; }

    /// <summary>
    ///     Moment of submission.
    /// </summary>
    public DateTimeOffset SubmittedAt { get; }

    private Contest(
        Statement statement,
        DateTimeOffset submittedAt)
    {
        Statement = statement;
        SubmittedAt = submittedAt;
    }

    /// <summary>
    ///     Creates an objection.
    /// </summary>
    public static Contest Of(
        Statement statement,
        DateTimeOffset submittedAt)
    {
        ArgumentNullException.ThrowIfNull(statement);

        return new Contest(statement, submittedAt);
    }
}