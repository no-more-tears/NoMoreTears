using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NoMoreTears.Shared.Providers.Hosting;

public static class ModuleRegistrar
{
    public static IServiceCollection AddModules(
         this IServiceCollection services,
         IConfiguration configuration,
         params IModule[] modules)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(modules);

        EnsureUniqueNames(modules);
        foreach (var module in modules)
        {
            try
            {
                module.Register(services, configuration);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Module '{module.Name}' failed to register.", ex);
            }
        }

        return services;
    }

    private static void EnsureUniqueNames(IModule[] modules)
    {
        var duplicate = modules
            .GroupBy(m => m.Name, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(g => g.Count() > 1);

        if (duplicate is not null)
        {
            throw new InvalidOperationException($"Duplicate module name: '{duplicate.Key}'.");
        }
    }
}