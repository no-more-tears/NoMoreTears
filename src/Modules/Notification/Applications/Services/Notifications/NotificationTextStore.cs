using System.Collections.Concurrent;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NoMoreTears.Modules.Notification.Applications.Services.Notifications;

/// <summary>
///     Title and body of an in-app notification in one culture.
/// </summary>
public sealed record NotificationText(string Title, string Body);

/// <summary>
///     Renders texts of in-app notifications from the embedded catalogs <c>Templates/Notifications/{culture}.json</c>.
///     <c>{{key}}</c> is replaced with the data value as is, <c>{{key:t}}</c> with its translation from <c>values</c>.
/// </summary>
public static partial class NotificationTextStore
{
    private const string FallbackCulture = "en";

    private static readonly Assembly Assembly = typeof(NotificationTextStore).Assembly;
    private static readonly string ResourcePrefix = $"{Assembly.GetName().Name}.Templates.Notifications";
    private static readonly ConcurrentDictionary<string, Catalog?> Catalogs = new(StringComparer.OrdinalIgnoreCase);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    ///     Renders the text of <paramref name="type"/> in <paramref name="culture"/>, falling back to English.
    /// </summary>
    public static NotificationText Render(
        string type,
        string culture,
        IReadOnlyDictionary<string, string> data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var catalog = Load(culture) ?? Load(FallbackCulture)
            ?? throw new InvalidOperationException("No notification text catalog is embedded.");

        if (!catalog.Texts.TryGetValue(type, out var text))
        {
            throw new InvalidOperationException($"Notification type '{type}' has no text in culture '{culture}'.");
        }

        return new NotificationText(
            Substitute(text.Title, catalog, data),
            Substitute(text.Body, catalog, data));
    }

    private static string Substitute(
        string template,
        Catalog catalog,
        IReadOnlyDictionary<string, string> data)
    {
        return Placeholder().Replace(template, match =>
        {
            var value = data.TryGetValue(match.Groups["key"].Value, out var found) ? found : string.Empty;

            return match.Groups["translate"].Success && catalog.Values.TryGetValue(value, out var translated)
                ? translated
                : value;
        });
    }

    private static Catalog? Load(string culture)
    {
        return Catalogs.GetOrAdd(culture, name =>
        {
            using var stream = Assembly.GetManifestResourceStream($"{ResourcePrefix}.{name.ToLowerInvariant()}.json");
            return stream is null ? null : JsonSerializer.Deserialize<Catalog>(stream, JsonOptions);
        });
    }

    [GeneratedRegex(@"\{\{(?<key>[A-Za-z0-9]+)(?<translate>:t)?\}\}", RegexOptions.CultureInvariant)]
    private static partial Regex Placeholder();

    private sealed record TextTemplate(string Title, string Body);

    private sealed record Catalog(
        Dictionary<string, string> Values,
        Dictionary<string, TextTemplate> Texts);
}