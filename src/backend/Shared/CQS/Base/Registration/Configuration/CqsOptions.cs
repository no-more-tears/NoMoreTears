using System.Reflection;

namespace HauteCouture.Shared.CQS.Registration.Configuration;

/// <summary>
///     Configuration options for registering CQS handlers.
/// </summary>
public sealed class CqsOptions
{
    /// <summary>
    ///     Assembly containing CQS handlers to register.
    /// </summary>
    internal Assembly? Assembly { get; private set; }

    /// <summary>
    ///     Indicates whether logging behavior is enabled in the pipeline.
    /// </summary>
    internal bool LoggingEnabled { get; private set; }

    /// <summary>
    ///     Indicates whether performance behavior is enabled in the pipeline.
    /// </summary>
    internal bool PerformanceEnabled { get; private set; }

    /// <summary>
    ///     Indicates whether caching behavior is enabled in the pipeline.
    /// </summary>
    internal bool CachingEnabled { get; private set; }

    /// <summary>
    ///     Indicates whether validation behavior is enabled in the pipeline.
    /// </summary>
    internal bool ValidationEnabled { get; private set; }

    /// <summary>
    ///     Indicates whether authorization behavior is used.
    /// </summary>
    internal bool AuthorizationEnabled { get; private set; }

    /// <summary>
    ///     Indicates whether diagnostic/tracing behavior is enabled in the pipeline.
    /// </summary>
    internal bool DiagnosticEnabled { get; private set; }

    /// <summary>
    ///     Specifies the assembly to scan for CQS handlers.
    /// </summary>
    /// <param name="assembly">Assembly.</param>
    /// <returns>Current <see cref="CqsOptions"/> instance for fluent configuration.</returns>
    public CqsOptions FromAssembly(Assembly? assembly)
    {
        Assembly = assembly;
        return this;
    }

    /// <summary>
    ///     Specifies the assembly to scan for CQS handlers based on a marker type.
    /// </summary>
    /// <typeparam name="TMarker">Type defined in the assembly to scan.</typeparam>
    /// <returns>The current <see cref="CqsOptions"/> instance for fluent configuration.</returns>
    public CqsOptions FromAssembly<TMarker>()
    {
        Assembly = typeof(TMarker).Assembly;
        return this;
    }

    /// <summary>
    ///     Enables logging behavior in the CQS pipeline.
    /// </summary>
    /// <returns>The current <see cref="CqsOptions"/> instance for fluent configuration.</returns>
    public CqsOptions UseLogging()
    {
        LoggingEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables performance tracking behavior in the CQS pipeline.
    /// </summary>
    /// <returns>The current <see cref="CqsOptions"/> instance for fluent configuration.</returns>
    public CqsOptions UsePerformanceTracking()
    {
        PerformanceEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables caching behavior in the CQS pipeline.
    /// </summary>
    /// <returns>The current <see cref="CqsOptions"/> instance for fluent configuration.</returns>
    public CqsOptions UseCaching()
    {
        CachingEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables validation behavior in the CQS pipeline.
    /// </summary>
    /// <returns>The current <see cref="CqsOptions"/> instance for fluent configuration.</returns>
    public CqsOptions UseValidation()
    {
        ValidationEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables authorization behavior in the CQS pipeline.
    /// </summary>
    /// <returns>The current <see cref="CqsOptions"/> instance for fluent configuration.</returns>
    public CqsOptions UseAuthorization()
    {
        AuthorizationEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables diagnostic/tracing behavior in the CQS pipeline.
    /// </summary>
    /// <returns>The current <see cref="CqsOptions"/> instance for fluent configuration.</returns>
    public CqsOptions UseDiagnostics()
    {
        DiagnosticEnabled = true;
        return this;
    }

    /// <summary>
    ///     Enables all available pipeline behaviors at once.
    /// </summary>
    /// <returns>The current <see cref="CqsOptions"/> instance for fluent configuration.</returns>
    public CqsOptions UseAllBehaviors()
    {
        return UseDiagnostics()
            .UseAuthorization()
            .UseValidation()
            .UseLogging()
            .UsePerformanceTracking()
            .UseCaching();
    }
}