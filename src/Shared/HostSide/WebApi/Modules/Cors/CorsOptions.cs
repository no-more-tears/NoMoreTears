namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Cors;

/// <summary>
///     Configuration options for cross-origin requests from browser clients.
/// </summary>
public sealed class CorsOptions
{
    public const string SectionName = "Cors";

    /// <summary>
    ///     Origins allowed to call the API, for example <c>https://app.example.com</c>.
    ///     Cross-origin requests are rejected when the list is empty.
    /// </summary>
    public string[] AllowedOrigins { get; init; } = [];

    /// <summary>
    ///     Allowed http methods. Any method is allowed when the list is empty.
    /// </summary>
    public string[] AllowedMethods { get; init; } = [];

    /// <summary>
    ///     Allowed request headers. Any header is allowed when the list is empty.
    /// </summary>
    public string[] AllowedHeaders { get; init; } = [];

    /// <summary>
    ///     Response headers readable by the browser client.
    /// </summary>
    public string[] ExposedHeaders { get; init; } =
    [
        "Retry-After",
        "X-RateLimit-Limit",
        "X-RateLimit-Remaining",
        "X-RateLimit-Reset"
    ];

    /// <summary>
    ///     Indicates whether cookies and authorization headers may be sent cross-origin.
    /// </summary>
    public bool AllowCredentials { get; init; } = true;

    /// <summary>
    ///     Duration in seconds the browser may cache a preflight response.
    /// </summary>
    public int PreflightMaxAgeInSeconds { get; init; } = 600;
}
