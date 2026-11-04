using System.Globalization;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Localization.Translations;

/// <summary>
///     Resolves culture names and fails fast when the runtime has no data for them.
/// </summary>
internal static class CultureResolver
{
    /// <summary>
    ///     Returns the culture with the given name.
    /// </summary>
    public static CultureInfo Resolve(string name, string source)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new WebModuleException($"{source}: culture name must not be empty.");
        }

        try
        {
            return CultureInfo.GetCultureInfo(name.Trim());
        }
        catch (CultureNotFoundException)
        {
            throw new WebModuleException(
                $"{source}: culture '{name}' is not available. Make sure ICU is installed and " +
                "DOTNET_SYSTEM_GLOBALIZATION_INVARIANT is not enabled.");
        }
    }
}