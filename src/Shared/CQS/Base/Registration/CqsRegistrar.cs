using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.CQS.Behaviors;
using NoMoreTears.Shared.CQS.Extensions;
using NoMoreTears.Shared.CQS.Registration.Configuration;

namespace NoMoreTears.Shared.CQS.Registration;

/// <summary>
///     Registrar for handlers and validators.
/// </summary>
public static class CqsRegistrar
{
    /// <param name="services">DI service collection.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers validators, command/query handlers from the specified assembly,
        ///     and configures the CQS pipeline behaviors.
        /// </summary>
        /// <param name="configureOptions">CQS configuration.</param>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddCqs(Action<CqsOptions> configureOptions)
        {
            var cqsOptions = new CqsOptions();
            configureOptions(cqsOptions);

            if (cqsOptions.Assembly is null)
            {
                throw new InvalidOperationException(
                    $"Assembly must be specified. Use {nameof(cqsOptions.FromAssembly)}() method in configuration.");
            }

            services
                .AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssembly(cqsOptions.Assembly);
                })
                .AddCqsHandlersFromAssembly(cqsOptions.Assembly);

            if (cqsOptions.AuthorizationEnabled)
            {
                services.AddAuthorizationBehavior();
            }
            if (cqsOptions.ValidationEnabled)
            {
                services.AddValidationBehavior();
            }
            if (cqsOptions.LoggingEnabled)
            {
                services.AddLoggingBehavior();
            }
            if (cqsOptions.PerformanceEnabled)
            {
                services.AddPerformanceBehavior();
            }
            if (cqsOptions.CachingEnabled)
            {
                services.AddCachingBehavior();
            }

            return services;
        }

        /// <summary>
        ///     Registers the logging pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddLoggingBehavior()
        {
            return services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        }

        /// <summary>
        ///     Registers the performance tracking pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddPerformanceBehavior()
        {
            return services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        }

        /// <summary>
        ///     Registers the caching pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddCachingBehavior()
        {
            return services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        }

        /// <summary>
        ///     Registers the validation pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddValidationBehavior()
        {
            return services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        /// <summary>
        ///     Registers the authorization pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddAuthorizationBehavior()
        {
            return services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        }
    }
}