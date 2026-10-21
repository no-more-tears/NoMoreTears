using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NoMoreTears.Shared.Providers.Hosting;

/// <summary>
///     Interface for represeting a module.
/// </summary>
public interface IModule
{
    /// <summary>
    ///     Module name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Registers the module's services.
    /// </summary>
    void Register(
        IServiceCollection services, 
        IConfiguration configuration);
}