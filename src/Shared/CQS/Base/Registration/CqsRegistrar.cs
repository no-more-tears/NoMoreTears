using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
            if (cqsOptions.TransactionsEnabled)
            {
                services.AddTransactionalBehavior();
            }

            return services;
        }

        /// <summary>
        ///     Registers the logging pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddLoggingBehavior()
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>)));
            return services;
        }

        /// <summary>
        ///     Registers the performance tracking pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddPerformanceBehavior()
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>)));
            return services;
        }

        /// <summary>
        ///     Registers the caching pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddCachingBehavior()
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>)));
            return services;
        }

        /// <summary>
        ///     Registers the validation pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddValidationBehavior()
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)));
            return services;
        }

        /// <summary>
        ///     Registers the transactional pipeline behavior. Register it after the other behaviors,
        ///     so that it stays the innermost one.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddTransactionalBehavior()
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>), typeof(TransactionalBehavior<,>)));
            return services;
        }

        /// <summary>
        ///     Registers the authorization pipeline behavior.
        /// </summary>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddAuthorizationBehavior()
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>)));
            return services;
        }
    }
}