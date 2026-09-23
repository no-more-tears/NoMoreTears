using NoMoreTears.Shared.Postgres.Configurations.Enums;

namespace NoMoreTears.Shared.Postgres.Configurations;

/// <summary>
///     Represents a blueprint for configuring a DbContext with various settings.
/// </summary>
public sealed class DbContextBlueprint
{
    /// <summary>
    ///     Database connection string name.
    /// </summary>
    public string ConnectionString { get; private set; } = string.Empty;

    /// <summary>
    ///    Maximum time that a database command is allowed to execute before timing out.
    /// </summary>
    public int CommandTimeout { get; private set; } = 30;

    /// <summary>
    ///     Maximum number of SQL statements sent to the database in a single round-trip during.
    /// </summary>
    public int MaxBatchSize { get; private set; } = 128;

    /// <summary>
    ///     Maximum delay between retries when a database operation fails.
    /// </summary>
    public TimeSpan MaxRetryDelay { get; private set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    ///     Maximum delay between retry attempts for transient database failures. 
    /// </summary>
    public int MaxRetryCount { get; private set; } = 5;

    /// <summary>
    ///     Minimum execution time required for a query to be considered slow. 
    /// </summary>
    public TimeSpan SlowQueryThreshold { get; private set; } = TimeSpan.FromMilliseconds(800);

    /// <summary>
    ///     Indicates whether sensitive data logging should be enabled.
    ///     Dev stage: true; Prod stage: false.
    /// </summary>
    public bool SensitiveDataLogging { get; private set; } = false;

    /// <summary>
    ///     Indicates whether detailed error messages should be enabled. 
    ///     Dev stage: true; Prod stage: false.
    /// </summary>
    public bool DetailedErrorsEnabled { get; private set; } = false;

    /// <summary>
    ///    Specifies the naming convention for database objects.
    /// </summary>
    public NamingStyle NamingStyle { get; private set; } = NamingStyle.Default;

    /// <summary>
    ///    Sets the database connection string for the DbContext.
    /// </summary>
    /// <returns>The configured blueprint.</returns>
    public DbContextBlueprint UseConnectionString(string connectionString)
    {
        ConnectionString = connectionString;
        return this;
    }

    /// <summary>
    ///     Sets the command timeout for database operations in seconds.
    /// </summary>
    public DbContextBlueprint WithCommandTimeout(int commandTimeout)
    {
        CommandTimeout = commandTimeout;
        return this;
    }

    /// <summary>
    ///     Sets the maximum batch size for database operations.
    /// </summary>
    public DbContextBlueprint WithMaxBatchSize(int maxBatchSize)
    {
        MaxBatchSize = maxBatchSize;
        return this;
    }

    /// <summary>
    ///     Configures retry behavior for transient database failures, 
    ///     specifying the maximum delay between retries and the maximum number of retry attempts.
    /// </summary>
    public DbContextBlueprint WithRetryOnFailure(
        TimeSpan maxRetryDelay, 
        int maxRetryCount)
    {
        MaxRetryDelay = maxRetryDelay;
        MaxRetryCount = maxRetryCount;

        return this;
    }

    /// <summary>
    ///     Sets the threshold for slow queries, 
    ///     specifying the minimum execution time required for a query to be considered slow.
    /// </summary>
    public DbContextBlueprint WithSlowQueryThreshold(TimeSpan slowQueryThreshold)
    {
        SlowQueryThreshold = slowQueryThreshold;
        return this;
    }

    /// <summary>
    ///     Enables or disables sensitive data logging for the DbContext,
    ///     allowing for more detailed logging of database operations.
    /// </summary>
    public DbContextBlueprint EnableSensitiveDataLogging(bool enableSensitiveDataLogging)
    {
        SensitiveDataLogging = enableSensitiveDataLogging;
        return this;
    }

    /// <summary>
    ///     Enables or disables detailed error messages for the DbContext, 
    ///     providing more information about database errors when they occur.
    /// </summary>
    public DbContextBlueprint EnableDetailedErrors(bool enableDetailedErrors)
    {
        DetailedErrorsEnabled = enableDetailedErrors;
        return this;
    }

    /// <summary>
    ///     Enables or disables verbose diagnostics for the DbContext.
    /// </summary>
    public DbContextBlueprint EnableVerboseDiagnostics(bool enableVerboseDiagnostics)
    {
        SensitiveDataLogging = enableVerboseDiagnostics;
        DetailedErrorsEnabled = enableVerboseDiagnostics;

        return this;
    }

    /// <summary>
    ///     Sets the naming convention for database objects.
    /// </summary>
    public DbContextBlueprint WithNamingStyle(NamingStyle namingStyle)
    {
        NamingStyle = namingStyle;
        return this;
    }
}