using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using NoMoreTears.Shared.Postgres.Configurations;
using NoMoreTears.Shared.Postgres.Registration;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Security;
using NoMoreTears.Shared.Providers.Identity.Authentication;
using NoMoreTears.Shared.Providers.Identity.Maintenance;
using NoMoreTears.Shared.Providers.Identity.Persistence;
using NoMoreTears.Shared.Providers.Identity.Registration.Configuration;

namespace NoMoreTears.Shared.Providers.Identity.Registration;

/// <summary>
///     Fluent configuration of the identity provider: options, persistence, replaceable
///     services and optional host integrations.
/// </summary>
/// <remarks>
///     Registrations are collected and applied by <see cref="IdentityRegistrar"/>
///     before the defaults, so every choice made here takes precedence.
/// </remarks>
public sealed class IdentityProviderBuilder
{
    private readonly List<Action<IServiceCollection>> _registrations = [];

    internal IdentityProviderBuilder()
    {
    }

    /// <summary>
    ///     Indicates whether a persistence registration has been requested.
    /// </summary>
    internal bool PersistenceConfigured { get; private set; }

    /// <summary>
    ///     Binds every option class from the sub-sections of the given configuration section
    ///     (<c>Jwt</c>, <c>Sessions</c>, <c>Mfa</c>, <c>UserActionTokens</c>, <c>Passwords</c>, <c>Account</c>).
    /// </summary>
    public IdentityProviderBuilder Configure(IConfiguration section)
    {
        ArgumentNullException.ThrowIfNull(section);

        return Register(services =>
        {
            services.AddOptions<JwtOptions>().Bind(section.GetSection(JwtOptions.SectionName));
            services.AddOptions<SessionOptions>().Bind(section.GetSection(SessionOptions.SectionName));
            services.AddOptions<MfaOptions>().Bind(section.GetSection(MfaOptions.SectionName));
            services.AddOptions<UserActionTokenOptions>().Bind(section.GetSection(UserActionTokenOptions.SectionName));
            services.AddOptions<PasswordOptions>().Bind(section.GetSection(PasswordOptions.SectionName));
            services.AddOptions<AccountOptions>().Bind(section.GetSection(AccountOptions.SectionName));
        });
    }

    /// <summary>
    ///     Configures JWT access token options in code.
    /// </summary>
    public IdentityProviderBuilder ConfigureJwt(Action<JwtOptions> configure) => ConfigureOptions(configure);

    /// <summary>
    ///     Configures session and refresh token options in code.
    /// </summary>
    public IdentityProviderBuilder ConfigureSessions(Action<SessionOptions> configure) => ConfigureOptions(configure);

    /// <summary>
    ///     Configures multi-factor authentication options in code.
    /// </summary>
    public IdentityProviderBuilder ConfigureMfa(Action<MfaOptions> configure) => ConfigureOptions(configure);

    /// <summary>
    ///     Configures user action token options in code.
    /// </summary>
    public IdentityProviderBuilder ConfigureUserActionTokens(Action<UserActionTokenOptions> configure) => ConfigureOptions(configure);

    /// <summary>
    ///     Configures password hashing and policy options in code.
    /// </summary>
    public IdentityProviderBuilder ConfigurePasswords(Action<PasswordOptions> configure) => ConfigureOptions(configure);

    /// <summary>
    ///     Configures account policy options in code.
    /// </summary>
    public IdentityProviderBuilder ConfigureAccount(Action<AccountOptions> configure) => ConfigureOptions(configure);

    /// <summary>
    ///     Stores identity data in Postgres through the Postgres module, drafting the connection
    ///     and tuning blueprint inline.
    /// </summary>
    /// <remarks>
    ///     Requires <see cref="IConfiguration"/> and <c>ILoggerFactory</c> to be registered by the host.
    /// </remarks>
    public IdentityProviderBuilder UsePostgres(
        Action<DbContextBlueprint> draft,
        int poolSize = PostgresRegistrar.DbContextPoolSize)
    {
        ArgumentNullException.ThrowIfNull(draft);

        return UsePersistence(services => services
            .AddSingleton(new IdentityDbContextDraft(draft))
            .AddSingleton<IDbContextConfigurator<IdentityDbContext>, IdentityDbContextForge>()
            .AddDbContextPool<IdentityDbContext>(ConfigureFromConfigurator, poolSize));
    }

    /// <summary>
    ///     Stores identity data in Postgres using a configurator supplied by the host,
    ///     typically a <see cref="DbContextForge{TDbContext}"/> for <see cref="IdentityDbContext"/>.
    /// </summary>
    public IdentityProviderBuilder UsePostgres<TConfigurator>(int poolSize = PostgresRegistrar.DbContextPoolSize)
        where TConfigurator : class, IDbContextConfigurator<IdentityDbContext>
    {
        return UsePersistence(services => services
            .AddSingleton<IDbContextConfigurator<IdentityDbContext>, TConfigurator>()
            .AddDbContextPool<IdentityDbContext>(ConfigureFromConfigurator, poolSize));
    }

    /// <summary>
    ///     Configures <see cref="IdentityDbContext"/> directly, bypassing the Postgres module.
    /// </summary>
    public IdentityProviderBuilder UseDbContext(
        Action<DbContextOptionsBuilder> configure,
        int poolSize = PostgresRegistrar.DbContextPoolSize)
    {
        ArgumentNullException.ThrowIfNull(configure);
        return UseDbContext((_, builder) => configure(builder), poolSize);
    }

    /// <summary>
    ///     Configures <see cref="IdentityDbContext"/> directly with access to the service provider.
    /// </summary>
    public IdentityProviderBuilder UseDbContext(
        Action<IServiceProvider, DbContextOptionsBuilder> configure,
        int poolSize = PostgresRegistrar.DbContextPoolSize)
    {
        ArgumentNullException.ThrowIfNull(configure);
        return UsePersistence(services => services.AddDbContextPool<IdentityDbContext>(configure, poolSize));
    }

    /// <summary>
    ///     Replaces the default BCrypt password hasher.
    /// </summary>
    public IdentityProviderBuilder UsePasswordHasher<THasher>()
        where THasher : class, IPasswordHasher
    {
        return Register(services => services.AddSingleton<IPasswordHasher, THasher>());
    }

    /// <summary>
    ///     Registers JWT bearer authentication for the default scheme, validated with the same options
    ///     the provider issues tokens with. Tokens of revoked sessions are rejected unless disabled.
    /// </summary>
    public IdentityProviderBuilder UseJwtBearer(
        Action<JwtBearerOptions>? configure = null,
        bool validateSessions = true)
    {
        return UseJwtBearer(JwtBearerDefaults.AuthenticationScheme, configure, validateSessions);
    }

    /// <summary>
    ///     Registers JWT bearer authentication for the given scheme, validated with the same options
    ///     the provider issues tokens with. Tokens of revoked sessions are rejected unless disabled.
    /// </summary>
    /// <remarks>
    ///     <paramref name="configure"/> runs after the provider defaults; assign handlers on the existing
    ///     <see cref="JwtBearerOptions.Events"/> instance to keep session validation in place.
    /// </remarks>
    public IdentityProviderBuilder UseJwtBearer(
        string scheme,
        Action<JwtBearerOptions>? configure = null,
        bool validateSessions = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scheme);

        return Register(services =>
        {
            services.AddAuthentication(scheme).AddJwtBearer(scheme);
            services.AddSingleton(new JwtBearerIntegration(scheme, validateSessions));
            services.TryAddEnumerable(
                ServiceDescriptor.Singleton<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsConfigurator>());

            if (configure is not null)
            {
                services.PostConfigure(scheme, configure);
            }
        });
    }

    /// <summary>
    ///     Runs identity maintenance periodically in a hosted service.
    /// </summary>
    /// <param name="interval">Period between runs; defaults to <see cref="MaintenanceOptions.Interval"/>.</param>
    public IdentityProviderBuilder UseBackgroundCleanup(TimeSpan? interval = null)
    {
        return Register(services =>
        {
            if (interval is { } value)
            {
                services.Configure<MaintenanceOptions>(options => options.Interval = value);
            }

            services.AddHostedService<IdentityCleanupWorker>();
        });
    }

    internal void Apply(IServiceCollection services)
    {
        foreach (var registration in _registrations)
        {
            registration(services);
        }
    }

    private IdentityProviderBuilder ConfigureOptions<TOptions>(Action<TOptions> configure)
        where TOptions : class
    {
        ArgumentNullException.ThrowIfNull(configure);
        return Register(services => services.Configure(configure));
    }

    private IdentityProviderBuilder UsePersistence(Action<IServiceCollection> registration)
    {
        if (PersistenceConfigured)
        {
            throw new InvalidOperationException("Identity persistence has already been configured.");
        }

        PersistenceConfigured = true;
        return Register(registration);
    }

    private IdentityProviderBuilder Register(Action<IServiceCollection> registration)
    {
        _registrations.Add(registration);
        return this;
    }

    private static void ConfigureFromConfigurator(
        IServiceProvider provider,
        DbContextOptionsBuilder builder)
    {
        provider
            .GetRequiredService<IDbContextConfigurator<IdentityDbContext>>()
            .Configure((DbContextOptionsBuilder<IdentityDbContext>)builder);
    }
}
