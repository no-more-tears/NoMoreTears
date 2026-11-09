using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.HostSide.SignalR.Registration.Configuration;
using NoMoreTears.Shared.HostSide.SignalR.Users;
using StackExchange.Redis;

namespace NoMoreTears.Shared.HostSide.SignalR.Registration;

/// <summary>
///     Registrar of the real-time infrastructure shared by SignalR hosts.
/// </summary>
public static class SignalRRegistrar
{
    private const string AccessTokenQueryParameter = "access_token";

    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers SignalR with the MessagePack protocol, the Redis backplane for scaling out, and user identification by the
        ///     subject of the access token.
        /// </summary>
        /// <param name="configuration">Root configuration; options are bound from <see cref="SignalROptions.SectionName"/>.</param>
        /// <param name="authenticationScheme">JWT bearer scheme that accepts the token from the query string on hub paths.</param>
        public IServiceCollection AddRealtime(
            IConfiguration configuration,
            string authenticationScheme = JwtBearerDefaults.AuthenticationScheme)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var options = configuration.GetSection(SignalROptions.SectionName).Get<SignalROptions>()
                ?? throw new InvalidOperationException($"Section '{SignalROptions.SectionName}' is not configured.");

            if (string.IsNullOrWhiteSpace(options.RedisConnectionString))
            {
                throw new InvalidOperationException(
                    $"'{SignalROptions.SectionName}:{nameof(SignalROptions.RedisConnectionString)}' is required for the backplane.");
            }

            services
                .AddSignalR(hub =>
                {
                    hub.KeepAliveInterval = options.KeepAliveInterval;
                    hub.ClientTimeoutInterval = options.ClientTimeoutInterval;
                    hub.MaximumReceiveMessageSize = options.MaximumReceiveMessageSize;
                    hub.EnableDetailedErrors = false;
                })
                .AddMessagePackProtocol()
                .AddStackExchangeRedis(redis =>
                {
                    redis.Configuration = ConfigurationOptions.Parse(options.RedisConnectionString);
                    redis.Configuration.AbortOnConnectFail = false;
                    redis.Configuration.ChannelPrefix = RedisChannel.Literal(options.ChannelPrefix);

                    if (!string.IsNullOrWhiteSpace(options.RedisPassword))
                    {
                        redis.Configuration.Password = options.RedisPassword;
                    }
                });

            services.AddSingleton<IUserIdProvider, SubjectUserIdProvider>();
            services.PostConfigure<JwtBearerOptions>(authenticationScheme, jwt => AcceptQueryToken(jwt, options.HubsPathPrefix));

            return services;
        }
    }

    /// <summary>
    ///     Reads the access token from the query string for hub requests, keeping the handler configured before.
    /// </summary>
    private static void AcceptQueryToken(
        JwtBearerOptions jwt,
        string hubsPathPrefix)
    {
        jwt.Events ??= new JwtBearerEvents();
        var next = jwt.Events.OnMessageReceived;

        jwt.Events.OnMessageReceived = async context =>
        {
            var accessToken = context.Request.Query[AccessTokenQueryParameter].ToString();

            if (!string.IsNullOrEmpty(accessToken)
                && context.HttpContext.Request.Path.StartsWithSegments(hubsPathPrefix, StringComparison.OrdinalIgnoreCase))
            {
                context.Token = accessToken;
            }

            if (next is not null)
            {
                await next(context);
            }
        };
    }
}