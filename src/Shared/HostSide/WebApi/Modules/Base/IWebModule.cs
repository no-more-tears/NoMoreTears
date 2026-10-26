using Microsoft.AspNetCore.Builder;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Base;

/// <summary>
///     Represents a self-contained web infrastructure module that configures
///     both DI services and the HTTP middleware pipeline.
/// </summary>
public interface IWebModule
{
    /// <summary>
    ///     Defines the execution order of this module in the pipeline.
    ///     Lower values mount earlier.
    /// </summary>
    int Order { get; }

    /// <summary>
    ///     Registers services required by this module into the DI container.
    /// </summary>
    void MountServices(WebModuleContext context);

    /// <summary>
    ///     Configures the HTTP middleware pipeline for this module.
    /// </summary>
    void MountPipeline(IApplicationBuilder app);
}