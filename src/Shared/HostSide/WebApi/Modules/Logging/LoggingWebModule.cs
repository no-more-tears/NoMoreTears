using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using System.Globalization;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Logging;

/// <summary>
///     Web module responsible for logging.
/// </summary>
public sealed class LoggingWebModule : IWebModule
{
    private const string ServiceNameProperty = "ServiceName";

    /// <inheritdoc />
    public int Order => WebModuleOrder.Logging;

    /// <inheritdoc />
    public void MountServices(WebModuleContext context)
    {
        context.Host
            .ConfigureLogging(logger =>
            {
                logger.ClearProviders();
            })
            .UseSerilog((loggerContext, loggerConfiguration) =>
            {
                if (HasSerilogSection(context.Configuration))
                {
                    loggerConfiguration
                        .ReadFrom.Configuration(loggerContext.Configuration);
                }
                else
                {
                    ConfigureSerilog(loggerConfiguration, context.Configuration);
                }

                loggerConfiguration
                    .Enrich.WithProperty(ServiceNameProperty, GetServiceName(context));
            });

        // For Serilog internal debugging.
        if (context.Environment.IsDevelopment())
        {
            SelfLog.Enable(Console.Out);
        }
    }

    /// <inheritdoc />
    public void MountPipeline(IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
                diagnosticContext.Set("ClientIp", httpContext.Connection.RemoteIpAddress?.ToString());
            };
        });
    }

    private static string GetServiceName(WebModuleContext context)
    {
        return context.Configuration["Serilog:Properties:ServiceName"]
            ?? context.Environment.ApplicationName;
    }

    private static bool HasSerilogSection(IConfiguration configuration)
    {
        return configuration
            .GetSection("Serilog")
            .Exists();
    }

    private static void ConfigureSerilog(
        LoggerConfiguration loggerConfiguration,
        IConfiguration configuration)
    {
        // TODO: Debug for Development, Info or Waring for Production.
        loggerConfiguration
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Debug)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .WriteTo.Console(
                formatProvider: CultureInfo.InvariantCulture,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] {Message:lj}{NewLine}{Exception}");

        var seqUrl = configuration["Seq:ServerUrl"];

        if (!string.IsNullOrWhiteSpace(seqUrl))
        {
            loggerConfiguration.WriteTo.Seq(seqUrl);
        }
    }
}