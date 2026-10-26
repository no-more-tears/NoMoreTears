using NoMoreTears.Shared.HostSide.Endpoints.Extensions;
using NoMoreTears.Shared.HostSide.Endpoints.Registration;
using NoMoreTears.Shared.HostSide.WebApi.Registration;

namespace NoMoreTears.HostSide.WebApi.Registration;

/// <summary>
///     Provides extension methods for configuring the WebApi
///     HTTP request pipeline.
/// </summary>
public static class HostPipelineRegistrar
{
    private const string ApiPrefix = "/api";

    extension (WebApplication application)
    {
        /// <summary>
        ///     Configures the HTTP request pipeline.
        /// </summary>
        public WebApplication UseWebApiHostPipeline()
        {
            if (application.Environment.IsDevelopment())
            {
                application.MapOpenApi();
            }

            application
                .UseHttpsRedirection()
                .UseWebModules();

            // Every API endpoint shares one per-client budget; sensitive endpoints add their own limits on top.
            application
                .MapGroup(ApiPrefix)
                .WithRateLimit()
                .WithThrottle()
                .MapEndpoints();

            return application;
        }
    }
}