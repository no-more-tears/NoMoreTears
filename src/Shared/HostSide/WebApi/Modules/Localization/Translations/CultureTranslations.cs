using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using NoMoreTears.Shared.HostSide.WebApi.Modules.Base;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Localization.Translations;

/// <summary>
///     Translations of one culture: exact messages and templates with variable parts.
/// </summary>
internal sealed class CultureTranslations(CultureInfo culture)
{
    private readonly Dictionary<string, string> _exact = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _templateKeys = new(StringComparer.Ordinal);
    private readonly List<TranslationTemplate> _templates = [];

    /// <summary>
    ///     Adds a translation.
    ///     The same key may appear in several catalog files only with the same translation.
    /// </summary>
    public void Add(string key, string translation, string source)
    {
        var isTemplate = TranslationTemplate.IsTemplate(key);
        var keys = isTemplate ? _templateKeys : _exact;

        if (keys.TryGetValue(key, out var existing))
        {
            if (!string.Equals(existing, translation, StringComparison.Ordinal))
            {
                throw new WebModuleException($"Translation of '{key}' in {source} conflicts with an earlier translation.");
            }

            return;
        }

        keys.Add(key, translation);

        if (isTemplate)
        {
            try
            {
                _templates.Add(TranslationTemplate.Create(key, translation, culture));
            }
            catch (FormatException exception)
            {
                throw new WebModuleException($"{source}: {exception.Message}");
            }

            // Wrapping templates go first, then more specific ones.
            _templates.Sort((left, right) => right.StartsWithLiteral != left.StartsWithLiteral
                ? right.StartsWithLiteral.CompareTo(left.StartsWithLiteral)
                : right.LiteralLength.CompareTo(left.LiteralLength));
        }
    }

    /// <summary>
    ///     Finds the translation of a single-line text.
    /// </summary>
    public bool TryTranslate(string text, Func<string, string> translate, [NotNullWhen(true)] out string? result)
    {
        if (_exact.TryGetValue(text, out result))
        {
            return true;
        }

        foreach (var template in _templates)
        {
            if (template.TryApply(text, translate, out result))
            {
                return true;
            }
        }

        result = null;
        return false;
    }
}