using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Authorization;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Caching;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Cors;
using NoMoreTears.Shared.HostSide.WebApi.Modules.ExceptionHandling;
using NoMoreTears.Shared.HostSide.WebApi.Modules.HealthCheck;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Logging;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Swagger;
using NoMoreTears.Shared.HostSide.WebApi.Modules.TrafficControl;
using NoMoreTears.Shared.HostSide.WebApi.Registration.Configuration;
using System.Reflection;

namespace NoMoreTears.Shared.HostSide.WebApi.Registration;

/// <summary>
///     Registrar for discovering and mounting web modules defined
///     by <see cref="IWebModule"/> implementations.
/// </summary>
public static class WebApiRegistrar
{
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers <see cref="IWebModule"/> implementations based on the provided options.
        /// </summary>
        public IServiceCollection AddWebModules(
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IHostBuilder host,
            Action<WebApiOptions> configureOptions)
        {
            var options = new WebApiOptions
            {
                Configuration = configuration,
                Environment = environment,
                Host = host
            };

            configureOptions(options);

            var context = new WebModuleContext
            {
                Services = services,
                Configuration = configuration,
                Environment = environment,
                Host = host
            };

            var allModules = BuildModuleList(options);

            foreach (var module in allModules)
            {
                module.MountServices(context);
            }

            services.AddSingleton<IReadOnlyList<IWebModule>>(allModules);
            return services;
        }

        /// <summary>
        ///     Registers the logging web module.
        /// </summary>
        public IServiceCollection AddLoggingModule(
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IHostBuilder host)
        {
            return services.AddWebModule(new LoggingWebModule(), configuration, environment, host);
        }

        /// <summary>
        ///     Registers the caching web module.
        /// </summary>
        public IServiceCollection AddCachingModule(
            IConfiguration configuration,
            IWebHostEnvironment environment, 
            IHostBuilder host)
        {
            return services.AddWebModule(new CachingWebModule(), configuration, environment, host);
        }

        /// <summary>
        ///     Registers the traffic control web module.
        /// </summary>
        public IServiceCollection AddTrafficControlModule(
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IHostBuilder host)
        {
            return services.AddWebModule(new TrafficControlWebModule(), configuration, environment, host);
        }

        /// <summary>
        ///     Registers the exception handling web module.
        /// </summary>
        public IServiceCollection AddExceptionHandlingModule(
            IConfiguration configuration,
            IWebHostEnvironment environment, 
            IHostBuilder host)
        {
            return services.AddWebModule(new ExceptionHandlingWebModule(), configuration, environment, host);
        }

        private IServiceCollection AddWebModule(
            IWebModule module,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IHostBuilder host)
        {
            var context = new WebModuleContext
            {
                Services = services,
                Configuration = configuration,
                Environment = environment,
                Host = host
            };

            module.MountServices(context);

            // Append to existing registered module list if present, otherwise create.
            var descriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(IReadOnlyList<IWebModule>));

            var existing = descriptor is { ImplementationInstance: IReadOnlyList<IWebModule> list }
                ? list.ToList()
                : [];

            if (existing.All(m => m.GetType() != module.GetType()))
            {
                existing.Add(module);
            }

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddSingleton<IReadOnlyList<IWebModule>>(
                existing.OrderBy(m => m.Order).ToList());

            return services;
        }
    }

    /// <summary>
    ///     Mounts all registered web module pipelines in their defined order.
    /// </summary>
    public static IApplicationBuilder UseWebModules(this IApplicationBuilder app)
    {
        var modules = app.ApplicationServices
            .GetRequiredService<IReadOnlyList<IWebModule>>();

        foreach (var module in modules)
        {
            module.MountPipeline(app);
        }

        return app;
    }

    private static List<IWebModule> BuildModuleList(WebApiOptions options)
    {
        var modules = new List<IWebModule>();

        if (options.LoggingEnabled)
        {
            // Adds the logging web module. Requires calling the UseLogging() method.
            modules.Add(new LoggingWebModule());
        }
        if (options.CachingEnabled)
        {
            // Adds the caching web module. Requires calling the UseCaching() method.
            modules.Add(new CachingWebModule());
        }
        if (options.CorsEnabled)
        {
            // Adds the CORS web module. Requires calling the UseCors() method.
            modules.Add(new CorsWebModule());
        }
        if (options.TrafficControlEnabled)
        {
            // Adds the traffic control web module. Requires calling the UseTrafficControl() method.
            modules.Add(new TrafficControlWebModule());
        }
        if (options.ExceptionHandlingEnabled)
        {
            // Adds the global exception handling web module. Requires calling the UseExceptionHandling() method.
            modules.Add(new ExceptionHandlingWebModule());
        }
        if (options.AuthorizationEnabled)
        {
            // Adds the user session web module. Requires calling the UseUserSessions() method.
            modules.Add(new AuthorizationWebModule());
        }
        if (options.SwaggerEnabled)
        {
            // Adds the Swagger web module. Requires calling the UseSwagger() method.
            modules.Add(new SwaggerWebModule());
        }
        if (options.HealthCheckEnabled)
        {
            // Adds the health check web module. Requires calling the UseHealthChecks() method.
            modules.Add(new HealthCheckWebModule(options.HealthCheckOptions!));
        }

        if (options.Assemblies.Length > 0)
        {
            var discoveredModules = DiscoverModules(options.Assemblies);
            modules.AddRange(discoveredModules);
        }

        modules.AddRange(options.ExplicitModules);

        return modules.DistinctBy(m => m.GetType()).OrderBy(m => m.Order).ToList();
    }

    private static IEnumerable<IWebModule> DiscoverModules(Assembly[] assemblies)
    {
        return assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                && t.IsAssignableTo(typeof(IWebModule))
                && t.GetConstructor(Type.EmptyTypes) is not null)
            .Select(t => (IWebModule)Activator.CreateInstance(t)!);
    }
}