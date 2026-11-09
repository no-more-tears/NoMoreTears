using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NoMoreTears.Modules.Masters.Applications.Services;
using NoMoreTears.Modules.Masters.Applications.Services.Abstractions;
using NoMoreTears.Modules.Masters.Applications.Services.Configuration;
using NoMoreTears.Modules.Masters.Applications.Services.Messaging;
using NoMoreTears.Modules.Masters.Domain.Aggregates;
using NoMoreTears.Modules.Masters.Infrastructures.DataAccess.Configurators;
using NoMoreTears.Modules.Masters.Infrastructures.DataAccess.DbContexts;
using NoMoreTears.Shared.CQS.Registration;
using NoMoreTears.Shared.FileStorage.Registration;
using NoMoreTears.Shared.HostSide.Endpoints.Registration;
using NoMoreTears.Shared.Postgres.Configurations;
using NoMoreTears.Shared.Postgres.Extensions;
using NoMoreTears.Shared.Postgres.Registration;
using NoMoreTears.Shared.RabbitMq.Registration;
using NoMoreTears.Shared.RabbitMq.Registration.Configuration;
using HandlersAssembly = NoMoreTears.Modules.Masters.Applications.Handlers.AssemblyReference;

namespace NoMoreTears.Modules.Masters.Providers;

/// <summary>
///     Registrar for the Masters module: every layer can be registered separately or all together.
/// </summary>
public static class MastersRegistrar
{
    /// <summary>
    ///     Root configuration section of the module.
    /// </summary>
    public const string SectionName = "Masters";

    /// <summary>
    ///     Prefix of the RabbitMQ queues of the module.
    /// </summary>
    public const string EndpointPrefix = "masters";

    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers data access: <see cref="MastersDbContext"/>.
        /// </summary>
        public IServiceCollection AddMastersDataAccess(Action<DbContextBlueprint> draft)
        {
            ArgumentNullException.ThrowIfNull(draft);

            services
                .AddSingleton(new MastersDbContextDraft(draft))
                .AddModulePostgres<MastersDbContext, MastersDbContextForge>(postgres => postgres
                    .AddRepository<Master, Guid>()
                    .AddRepository<Category, Guid>()
                    .AddRepository<FileUpload, Guid>()
                    .AddTransactionalScope(HandlersAssembly.Assembly));

            return services;
        }

        /// <summary>
        ///     Registers application services and the object storage.
        /// </summary>
        /// <param name="configuration">Root configuration (module section and <c>FileStorageSettings</c>).</param>
        public IServiceCollection AddMastersServices(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            services
                .AddOptions<MastersStorageOptions>()
                .Bind(configuration.GetSection(SectionName).GetSection(MastersStorageOptions.SectionName));

            services.TryAddSingleton(TimeProvider.System);

            services
                .AddFileStorage(configuration)
                .AddScoped<IMasterService, MasterService>()
                .AddScoped<IVerificationService, VerificationService>()
                .AddScoped<ISanctionService, SanctionService>()
                .AddScoped<ICategoryService, CategoryService>()
                .AddScoped<IFileUploadService, FileUploadService>();

            return services;
        }

        /// <summary>
        ///     Registers handlers with authorization, validation, performance tracking and transactions.
        /// </summary>
        /// <remarks>
        ///     Payload logging is not enabled: services log the outcome of each operation instead.
        /// </remarks>
        public IServiceCollection AddMastersHandlers()
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
        public IServiceCollection AddMastersApplications(IConfiguration configuration)
        {
            return services
                .AddMastersServices(configuration)
                .AddMastersHandlers();
        }

        /// <summary>
        ///     Registers HTTP endpoints of the module.
        /// </summary>
        public IServiceCollection AddMastersEndpoints()
        {
            return services.AddEndpoints(HostSide.Endpoints.AssemblyReference.Assembly);
        }

        /// <summary>
        ///     Describes the messaging of the module on the shared bus of the host.
        /// </summary>
        /// <param name="consumers">Consumers hosted by the caller; none in the WebApi host.</param>
        public IServiceCollection AddMastersMessaging(Action<RabbitMqModuleBuilder<IMastersOutbox>>? consumers = null)
        {
            return services.AddRabbitMqModule<IMastersOutbox>(EndpointPrefix, module =>
            {
                module.UsePostgresOutbox<IMastersOutbox, MastersDbContext>();
                consumers?.Invoke(module);
            });
        }
    }
}