using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Cors;

/// <summary>
///     Web module responsible for allowing cross-origin requests
///     from the origins listed in <see cref="CorsOptions"/>.
/// </summary>
public sealed class CorsWebModule : IWebModule
{
    private const string PolicyName = "ConfiguredOrigins";
    private const string AnyOrigin = "*";

    /// <inheritdoc />
    public int Order => WebModuleOrder.Cors;

    /// <inheritdoc />
    public void MountServices(WebModuleContext context)
    {
        var options = context.Configuration
            .GetSection(CorsOptions.SectionName)
            .Get<CorsOptions>()
            ?? new CorsOptions();

        var origins = options.AllowedOrigins
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin.Trim().TrimEnd('/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (origins.Contains(AnyOrigin) && options.AllowCredentials)
        {
            throw new WebModuleException(
                $"{CorsOptions.SectionName}: '{AnyOrigin}' cannot be combined with AllowCredentials; list the origins explicitly.");
        }

        context.Services.AddCors(cors => cors.AddPolicy(PolicyName, policy =>
        {
            if (origins.Contains(AnyOrigin))
            {
                policy.AllowAnyOrigin();
            }
            else
            {
                policy.WithOrigins(origins);
            }

            if (options.AllowedMethods.Length == 0)
            {
                policy.AllowAnyMethod();
            }
            else
            {
                policy.WithMethods(options.AllowedMethods);
            }

            if (options.AllowedHeaders.Length == 0)
            {
                policy.AllowAnyHeader();
            }
            else
            {
                policy.WithHeaders(options.AllowedHeaders);
            }

            if (options.AllowCredentials)
            {
                policy.AllowCredentials();
            }

            policy
                .WithExposedHeaders(options.ExposedHeaders)
                .SetPreflightMaxAge(TimeSpan.FromSeconds(options.PreflightMaxAgeInSeconds));
        }));
    }

    /// <inheritdoc />
    public void MountPipeline(IApplicationBuilder app)
    {
        // Mounted before exception handling and authorization, so preflight requests are answered
        // without authentication and error responses still carry the CORS headers.
        app.UseCors(PolicyName);
    }
}
