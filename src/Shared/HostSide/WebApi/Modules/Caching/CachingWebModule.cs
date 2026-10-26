using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;
using StackExchange.Redis;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Caching;

/// <summary>
///     Web module responsible for caching via Redis.
/// </summary>
public sealed class CachingWebModule : IWebModule
{
    /// <inheritdoc />
    public int Order => WebModuleOrder.Caching;

    /// <inheritdoc />
    public void MountServices(WebModuleContext context)
    {
        var options = context.Configuration
            .GetSection(CachingOptions.SectionName)
            .Get<CachingOptions>()
            ?? throw new WebModuleException(
                $"Missing required configuration section '{CachingOptions.SectionName}'.");

        var configurationOptions = new ConfigurationOptions
        {
            EndPoints = { options.ConnectionString },
            Password = options.Password,
            ClientName = options.InstanceName,
            AbortOnConnectFail = false,
        };

        context.Services
            .AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(configurationOptions))
            .AddStackExchangeRedisCache(redis =>
            {
                redis.ConfigurationOptions = configurationOptions;
                redis.InstanceName = options.InstanceName;
            });
    }

    /// <inheritdoc />
    public void MountPipeline(IApplicationBuilder app)
    {
    }
}