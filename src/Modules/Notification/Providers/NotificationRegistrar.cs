using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using NoMoreTears.Modules.Notification.Applications.Services;
using NoMoreTears.Modules.Notification.Applications.Services.Abstractions;
using NoMoreTears.Modules.Notification.Applications.Services.Configuration;
using NoMoreTears.Modules.Notification.Applications.Services.Messaging;
using NoMoreTears.Modules.Notification.Applications.Services.Realtime;
using NoMoreTears.Modules.Notification.Applications.Services.Smtp;
using NoMoreTears.Modules.Notification.Applications.Services.Templates;
using NoMoreTears.Modules.Notification.Domain.Aggregates;
using NoMoreTears.Modules.Notification.Infrastructures.DataAccess.Configurators;
using NoMoreTears.Modules.Notification.Infrastructures.DataAccess.DbContexts;
using NoMoreTears.Shared.CQS.Registration;
using NoMoreTears.Shared.HostSide.Endpoints.Registration;
using NoMoreTears.Shared.Postgres.Configurations;
using NoMoreTears.Shared.Postgres.Extensions;
using NoMoreTears.Shared.Postgres.Registration;
using NoMoreTears.Shared.RabbitMq.Registration;
using NoMoreTears.Shared.RabbitMq.Registration.Configuration;
using HandlersAssembly = NoMoreTears.Modules.Notification.Applications.Handlers.AssemblyReference;
using NotificationModel = NoMoreTears.Modules.Notification.Domain.Aggregates.Notification;

namespace NoMoreTears.Modules.Notification.Providers;

/// <summary>
///     Registrar for the Notification module.
/// </summary>
public static class NotificationRegistrar
{
    /// <summary>
    ///     Root configuration section of the module.
    /// </summary>
    public const string SectionName = "Notification";

    /// <summary>
    ///     Prefix of the RabbitMQ queues of the module.
    /// </summary>
    public const string EndpointPrefix = "notification";

    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers data access: <see cref="NotificationDbContext"/>.
        /// </summary>
        public IServiceCollection AddNotificationDataAccess(Action<DbContextBlueprint> draft)
        {
            ArgumentNullException.ThrowIfNull(draft);

            services
                .AddSingleton(new NotificationDbContextDraft(draft))
                .AddModulePostgres<NotificationDbContext, NotificationDbContextForge>(postgres => postgres
                    .AddRepository<NotificationModel, Guid>()
                    .AddRepository<EmailDelivery, Guid>()
                    .AddTransactionalScope(HandlersAssembly.Assembly));

            return services;
        }

        /// <summary>
        ///     Registers application services. Real-time delivery is a no-op unless the host registers its own
        ///     <see cref="IRealtimeNotifier"/> (the SignalR host does).
        /// </summary>
        /// <param name="configuration">Root configuration.</param>
        public IServiceCollection AddNotificationServices(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var section = configuration.GetSection(SectionName);

            services
                .AddOptions<InAppNotificationOptions>()
                .Bind(section.GetSection(InAppNotificationOptions.SectionName));

            services
                .AddOptions<SmtpOptions>()
                .Bind(section.GetSection(SmtpOptions.SectionName));

            services.TryAddSingleton(TimeProvider.System);
            services.TryAddSingleton<IEmailTemplateStore, EmbeddedEmailTemplateStore>();
            services.TryAddSingleton<IEmailNotifier, SmtpEmailNotifier>();
            services.TryAddSingleton<IRealtimeNotifier, NullRealtimeNotifier>();

            services
                .AddScoped<INotificationService, NotificationService>()
                .AddScoped<IEmailDeliveryService, EmailDeliveryService>();

            return services;
        }

        /// <summary>
        ///     Validates the SMTP options on start; call it in the host that delivers emails.
        /// </summary>
        public IServiceCollection AddNotificationEmailDelivery()
        {
            services
                .AddOptions<SmtpOptions>()
                .ValidateOnStart();

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<SmtpOptions>, SmtpOptionsValidator>());

            return services;
        }

        /// <summary>
        ///     Registers handlers with validation, performance tracking and transactions.
        /// </summary>
        public IServiceCollection AddNotificationHandlers()
        {
            return services.AddCqs(options => options
                .FromAssembly(HandlersAssembly.Assembly)
                .UseAuthorization()
                .UseValidation()
                .UsePerformanceTracking()
                .UseTransactions());
        }

        /// <summary>
        ///     Registers application services and handlers.
        /// </summary>
        public IServiceCollection AddNotificationApplications(IConfiguration configuration)
        {
            return services
                .AddNotificationServices(configuration)
                .AddNotificationHandlers();
        }

        /// <summary>
        ///     Registers HTTP endpoints of the module.
        /// </summary>
        public IServiceCollection AddNotificationEndpoints()
        {
            return services.AddEndpoints(HostSide.Endpoints.AssemblyReference.Assembly);
        }

        /// <summary>
        ///     Describes the messaging of the module on the shared bus of the host.
        ///     The host registers the bus itself with <c>AddRabbitMq</c> after all modules.
        /// </summary>
        /// <param name="consumers">Consumers hosted by the caller; none in the WebApi host.</param>
        public IServiceCollection AddNotificationMessaging(Action<RabbitMqModuleBuilder<INotificationOutbox>>? consumers = null)
        {
            return services.AddRabbitMqModule<INotificationOutbox>(EndpointPrefix, module =>
            {
                module.UsePostgresOutbox<INotificationOutbox, NotificationDbContext>();
                consumers?.Invoke(module);
            });
        }
    }
}