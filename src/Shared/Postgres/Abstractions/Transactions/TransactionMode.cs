namespace NoMoreTears.Shared.Postgres.Abstractions.Transactions;

/// <summary>
///     Defines how a command of a module is wrapped by the unit of work.
/// </summary>
public enum TransactionMode
{
    /// <summary>
    ///     The command runs inside a database transaction: all changes, including outbox messages,
    ///     are saved and committed together or rolled back together.
    /// </summary>
    Transaction,

    /// <summary>
    ///     The command runs without a transaction; pending changes (for example outbox messages)
    ///     are saved once after the command succeeds. Used by modules whose services must keep
    ///     their own changes even when the command fails afterwards (for example failed sign-in counters).
    /// </summary>
    SaveChanges
}
