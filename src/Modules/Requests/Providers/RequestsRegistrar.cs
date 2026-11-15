using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NoMoreTears.Modules.Requests.Applications.Services;
using NoMoreTears.Modules.Requests.Applications.Services.Abstractions;
using NoMoreTears.Modules.Requests.Applications.Services.Configuration;
using NoMoreTears.Modules.Requests.Applications.Services.Messaging;
using NoMoreTears.Modules.Requests.Domain.Aggregates;
using NoMoreTears.Modules.Requests.Infrastructures.DataAccess.Configurators;
using NoMoreTears.Modules.Requests.Infrastructures.DataAccess.DbContexts;
using NoMoreTears.Shared.CQS.Registration;
using NoMoreTears.Shared.FileStorage.Registration;
using NoMoreTears.Shared.HostSide.Endpoints.Registration;
using NoMoreTears.Shared.Postgres.Configurations;
using NoMoreTears.Shared.Postgres.Extensions;
using NoMoreTears.Shared.Postgres.Registration;
using NoMoreTears.Shared.RabbitMq.Registration;
using NoMoreTears.Shared.RabbitMq.Registration.Configuration;
using HandlersAssembly = NoMoreTears.Modules.Requests.Applications.Handlers.AssemblyReference;

namespace NoMoreTears.Modules.Requests.Providers;

/// <summary>
///     Registrar for the Requests module: every layer can be registered separately or all together.
/// </summary>
public static class RequestsRegistrar
{
    /// <summary>
    ///     Root configuration section of the module.
    /// </summary>
    public const string SectionName = "Requests";

    /// <summary>
    ///     Prefix of the RabbitMQ queues of the module.
    /// </summary>
    public const string EndpointPrefix = "requests";

    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers data access <see cref="RequestsDbContext"/>.
        /// </summary>
        public IServiceCollection AddRequestsDataAccess(Action<DbContextBlueprint> draft)
        {
            ArgumentNullException.ThrowIfNull(draft);

            services
                .AddSingleton(new RequestsDbContextDraft(draft))
                .AddModulePostgres<RequestsDbContext, RequestsDbContextForge>(postgres => postgres
                    .AddRepository<Request, Guid>()
                    .AddRepository<MediaUpload, Guid>()
                    .AddRepository<ClientRestriction, Guid>()
                    .AddRepository<MasterAccess, Guid>()
                    .AddRepository<ServiceCategory, Guid>()
                    .AddTransactionalScope(HandlersAssembly.Assembly));

            return services;
        }

        /// <summary>
        ///     Registers application services and the object storage.
        /// </summary>
        public IServiceCollection AddRequestsServices(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            services
                .AddOptions<RequestsStorageOptions>()
                .Bind(configuration.GetSection(SectionName).GetSection(RequestsStorageOptions.SectionName));

            services.TryAddSingleton(TimeProvider.System);

            services
                .AddFileStorage(configuration)
                .AddScoped<IRequestService, RequestService>()
                .AddScoped<IExecutionService, ExecutionService>()
                .AddScoped<IArbitrationService, ArbitrationService>()
                .AddScoped<IRestrictionService, RestrictionService>()
                .AddScoped<IMastersSnapshotService, MastersSnapshotService>()
                .AddScoped<IMediaUploadService, MediaUploadService>()
                .AddScoped<IDeadlineService, DeadlineService>();

            return services;
        }

        /// <summary>
        ///     Registers handlers with authorization, validation, performance tracking and transactions.
        /// </summary>
        public IServiceCollection AddRequestsHandlers()
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
        public IServiceCollection AddRequestsApplications(IConfiguration configuration)
        {
            return services
                .AddRequestsServices(configuration)
                .AddRequestsHandlers();
        }

        /// <summary>
        ///     Registers HTTP endpoints of the module.
        /// </summary>
        public IServiceCollection AddRequestsEndpoints()
        {
            return services.AddEndpoints(HostSide.Endpoints.AssemblyReference.Assembly);
        }

        /// <summary>
        ///     Describes the messaging of the module on the shared bus of the host.
        /// </summary>
        public IServiceCollection AddRequestsMessaging(Action<RabbitMqModuleBuilder<IRequestsOutbox>>? consumers = null)
        {
            return services.AddRabbitMqModule<IRequestsOutbox>(EndpointPrefix, module =>
            {
                module.UsePostgresOutbox<IRequestsOutbox, RequestsDbContext>();
                consumers?.Invoke(module);
            });
        }
    }
}