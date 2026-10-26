namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Base;

/// <summary>
///     Defines the default execution order constants for built-in web modules.
/// </summary>
public static class WebModuleOrder
{
    public const int Caching = 50;

    public const int Logging = 100;

    public const int Correlation = 200;

    public const int ExceptionHandling = 300;

    public const int OpenTelemetry = 400;

    public const int TrafficControl = 500;

    public const int Authorization = 600;

    public const int Swagger = 700;

    public const int HealthChecks = 800;
}