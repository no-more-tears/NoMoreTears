using System.Reflection;

namespace NoMoreTears.Shared.HostSide.WebApi;

/// <summary>
///     Reference to the assembly containing shared Web API components.
/// </summary>
public static class SharedWebApiAssemblyReference
{
    /// <summary>
    ///     Assembly reference.
    /// </summary>
    public static readonly Assembly Assembly = typeof(Assembly).Assembly;
}