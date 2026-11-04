using System.Globalization;
using System.Text.RegularExpressions;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Localization.Translations;

/// <summary>
///     Translates messages with the loaded catalogs. Multi-line messages, such as aggregated validation errors,
///     are translated line by line.
/// </summary>
internal sealed class MessageTranslator(TranslationCatalog catalog) : IMessageTranslator
{
    private const int MaxNestingDepth = 4;

    /// <inheritdoc />
    public string Translate(string text) => Translate(text, CultureInfo.CurrentUICulture);

    /// <inheritdoc />
    public string Translate(string text, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        if (string.IsNullOrEmpty(text))
        {
            return text;
        }

        var translations = catalog.Find(culture);

        if (translations is null)
        {
            return text;
        }

        if (!text.Contains('\n', StringComparison.Ordinal))
        {
            return TranslateLine(text, translations, 0);
        }

        var lines = text.Split('\n');

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var hasCarriageReturn = line.EndsWith('\r');
            var content = hasCarriageReturn ? line[..^1] : line;

            lines[index] = TranslateLine(content, translations, 0) + (hasCarriageReturn ? "\r" : string.Empty);
        }

        return string.Join('\n', lines);
    }

    private static string TranslateLine(string line, CultureTranslations translations, int depth)
    {
        if (line.Length == 0 || depth > MaxNestingDepth)
        {
            return line;
        }

        try
        {
            return translations.TryTranslate(line, part => TranslateLine(part, translations, depth + 1), out var translated)
                ? translated
                : line;
        }
        catch (RegexMatchTimeoutException)
        {
            return line;
        }
    }
}