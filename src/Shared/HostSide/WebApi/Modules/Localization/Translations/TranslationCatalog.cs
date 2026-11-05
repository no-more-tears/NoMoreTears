using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Localization.Translations;

/// <summary>
///     Translations loaded from JSON catalogs embedded as <c>Localization.{culture}.{area}.json</c>,
///     which come from the central folder <c>Modules/Localization/Catalogs/{culture}/{area}.json</c>.
///     A catalog is a flat object where keys are English messages and values are their translations.
/// </summary>
internal sealed partial class TranslationCatalog
{
    private readonly Dictionary<string, CultureTranslations> _cultures = new(StringComparer.OrdinalIgnoreCase);

    private TranslationCatalog()
    {
    }

    /// <summary>
    ///     Names of the cultures that have translations.
    /// </summary>
    public IReadOnlyCollection<string> Cultures => _cultures.Keys;

    /// <summary>
    ///     Loads the catalogs embedded into the assemblies.
    /// </summary>
    /// <exception cref="WebModuleException">A catalog is malformed or conflicts with another one.</exception>
    public static TranslationCatalog Load(IEnumerable<Assembly> assemblies)
    {
        var catalog = new TranslationCatalog();

        foreach (var assembly in assemblies.Distinct())
        {
            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                var match = ResourceName().Match(resourceName);

                if (match.Success)
                {
                    catalog.LoadResource(assembly, resourceName, match.Groups["culture"].Value);
                }
            }
        }

        return catalog;
    }

    /// <summary>
    ///     Returns the translations of the culture or of its closest parent, for example <c>uk</c> for <c>uk-UA</c>.
    /// </summary>
    public CultureTranslations? Find(CultureInfo culture)
    {
        for (var current = culture; !string.IsNullOrEmpty(current.Name); current = current.Parent)
        {
            if (_cultures.TryGetValue(current.Name, out var translations))
            {
                return translations;
            }
        }

        return null;
    }

    private void LoadResource(Assembly assembly, string resourceName, string cultureName)
    {
        var source = $"'{resourceName}' of '{assembly.GetName().Name}'";
        var culture = CultureResolver.Resolve(cultureName, source);

        if (!_cultures.TryGetValue(culture.Name, out var translations))
        {
            translations = new CultureTranslations(culture);
            _cultures.Add(culture.Name, translations);
        }

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new WebModuleException($"Translation catalog {source} cannot be read.");

        JsonDocument document;

        try
        {
            document = JsonDocument.Parse(stream, new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip });
        }
        catch (JsonException exception)
        {
            throw new WebModuleException($"Translation catalog {source} is not valid JSON: {exception.Message}");
        }

        using (document)
        {
            if (document.RootElement.ValueKind is not JsonValueKind.Object)
            {
                throw new WebModuleException($"Translation catalog {source} must be a JSON object.");
            }

            foreach (var entry in document.RootElement.EnumerateObject())
            {
                if (entry.Value.ValueKind is not JsonValueKind.String || string.IsNullOrWhiteSpace(entry.Value.GetString()))
                {
                    throw new WebModuleException($"Translation of '{entry.Name}' in {source} must be a non-empty string.");
                }

                translations.Add(entry.Name, entry.Value.GetString()!, source);
            }
        }
    }

    [GeneratedRegex(@"^Localization\.(?<culture>[A-Za-z]{2,8}(?:-[A-Za-z0-9]{1,8})*)(?:\.[A-Za-z0-9_-]+)?\.json$", RegexOptions.CultureInvariant)]
    private static partial Regex ResourceName();
}