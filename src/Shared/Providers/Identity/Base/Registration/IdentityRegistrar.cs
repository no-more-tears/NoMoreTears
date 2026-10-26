using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NoMoreTears.Shared.Providers.Identity.Abstractions;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Maintenance;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Security;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Services;
using NoMoreTears.Shared.Providers.Identity.Abstractions.Tokens;
using NoMoreTears.Shared.Providers.Identity.Maintenance;
using NoMoreTears.Shared.Providers.Identity.Registration.Configuration;
using NoMoreTears.Shared.Providers.Identity.Registration.Configuration.Validation;
using NoMoreTears.Shared.Providers.Identity.Security;
using NoMoreTears.Shared.Providers.Identity.Tokens;

namespace NoMoreTears.Shared.Providers.Identity.Registration;

/// <summary>
///     Registrar for the identity provider.
/// </summary>
public static class IdentityRegistrar
{
    /// <summary>
    ///     Root configuration section of the provider.
    /// </summary>
    public const string SectionName = "Identity";

    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers the identity provider.
        /// </summary>
        public IServiceCollection AddIdentityProvider(Action<IdentityProviderBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var builder = new IdentityProviderBuilder();
            configure(builder);

            if (!builder.PersistenceConfigured)
            {
                throw new InvalidOperationException(
                    $"Identity persistence must be configured. Use {nameof(IdentityProviderBuilder.UsePostgres)}() " +
                    $"or {nameof(IdentityProviderBuilder.UseDbContext)}() in configuration.");
            }

            builder.Apply(services);
            return services.AddIdentityDefaults();
        }

        /// <summary>
        ///     Registers the identity provider with configuration.
        /// </summary>
        public IServiceCollection AddIdentityProvider(
            IConfiguration configuration,
            Action<IdentityProviderBuilder> configure)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(configure);

            return services
                .AddIdentityProvider(builder =>
            {
                builder.Configure(configuration.GetSection(SectionName));
                configure(builder);
            });
        }

        /// <summary>
        ///     Registers identity provider services.
        /// </summary>
        private IServiceCollection AddIdentityDefaults()
        {
            services.TryAddSingleton(TimeProvider.System);

            services.AddValidatedOptions<JwtOptions, JwtOptionsValidator>();
            services.AddValidatedOptions<SessionOptions, SessionOptionsValidator>();
            services.AddValidatedOptions<MfaOptions, MfaOptionsValidator>();
            services.AddValidatedOptions<UserActionTokenOptions, UserActionTokenOptionsValidator>();
            services.AddValidatedOptions<PasswordOptions, PasswordOptionsValidator>();
            services.AddValidatedOptions<AccountOptions, AccountOptionsValidator>();
            services.AddValidatedOptions<MaintenanceOptions, MaintenanceOptionsValidator>();

            services
                .AddSingleton<IPasswordHasher, PasswordHasher>()
                .AddSingleton<DummyPasswordHash>()
                .AddSingleton<IJwtProvider, JwtProvider>()
                .AddScoped<IUserActionTokenService, UserActionTokenService>()
                .AddScoped<IMfaChallengeService, MfaChallengeService>()
                .AddScoped<IIdentityMaintenanceService, IdentityMaintenanceService>()
                .AddScoped<IdentityProvider>()
                .AddScoped<IIdentityProvider>(sp => sp.GetRequiredService<IdentityProvider>())
                .AddScoped<ISignUpService>(sp => sp.GetRequiredService<IdentityProvider>())
                .AddScoped<ISignInService>(sp => sp.GetRequiredService<IdentityProvider>())
                .AddScoped<IAccountService>(sp => sp.GetRequiredService<IdentityProvider>())
                .AddScoped<ISessionService>(sp => sp.GetRequiredService<IdentityProvider>())
                .AddScoped<IUserAdministrationService>(sp => sp.GetRequiredService<IdentityProvider>());

            return services;
        }

        /// <summary>
        ///     Registers options validated on application start by the given validator.
        /// </summary>
        private void AddValidatedOptions<TOptions, TValidator>()
            where TOptions : class
            where TValidator : class, IValidateOptions<TOptions>
        {
            services.AddOptions<TOptions>().ValidateOnStart();
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<TOptions>, TValidator>());
        }
    }

    /// <summary>
    ///     Creates <c>TokenValidationParameters</c> matching tokens issued with the given options,
    ///     for hosts that wire up bearer authentication themselves.
    /// </summary>
    public static TokenValidationParameters CreateTokenValidationParameters(JwtOptions options) =>
        JwtProvider.CreateValidationParameters(options);
}