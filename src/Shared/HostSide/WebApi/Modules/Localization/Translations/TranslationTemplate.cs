using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Localization.Translations;

/// <summary>
///     Translation of a message with variable parts. In the key, <c>{0}</c> matches any text.
/// </summary>
internal sealed partial class TranslationTemplate
{
    private static readonly TimeSpan MatchTimeout = TimeSpan.FromMilliseconds(100);

    private readonly Regex _pattern;
    private readonly string _translation;
    private readonly CultureInfo _culture;

    private TranslationTemplate(
        Regex pattern,
        string translation,
        CultureInfo culture,
        int literalLength,
        bool startsWithLiteral)
    {
        _pattern = pattern;
        _translation = translation;
        _culture = culture;
        LiteralLength = literalLength;
        StartsWithLiteral = startsWithLiteral;
    }

    /// <summary>
    ///     Number of fixed characters in the key; templates with more fixed text are more specific.
    /// </summary>
    public int LiteralLength { get; }

    /// <summary>
    ///     Whether the key starts with fixed text. Such templates usually wrap other messages,
    ///     for example <c>Field '{0}': {1}</c>, and are tried before templates that start with a placeholder.
    /// </summary>
    public bool StartsWithLiteral { get; }

    /// <summary>
    ///     Returns whether the key contains placeholders.
    /// </summary>
    public static bool IsTemplate(string key) => KeyPlaceholder().IsMatch(key);

    /// <summary>
    ///     Compiles a template from a key and its translation into the culture.
    /// </summary>
    public static TranslationTemplate Create(string key, string translation, CultureInfo culture)
    {
        var pattern = new StringBuilder("^");
        var placeholders = new HashSet<string>(StringComparer.Ordinal);
        var literalLength = 0;
        var position = 0;

        foreach (Match placeholder in KeyPlaceholder().Matches(key))
        {
            var literal = key[position..placeholder.Index];
            pattern.Append(Regex.Escape(literal));
            literalLength += literal.Length;

            var index = placeholder.Groups[1].Value;

            if (!placeholders.Add(index))
            {
                throw new FormatException($"Placeholder {{{index}}} is repeated in '{key}'.");
            }

            pattern.Append("(?<p").Append(index).Append(">.+?)");
            position = placeholder.Index + placeholder.Length;
        }

        var tail = key[position..];
        pattern.Append(Regex.Escape(tail)).Append('$');
        literalLength += tail.Length;

        EnsureTranslationPlaceholders(key, translation, placeholders, culture);

        var regex = new Regex(pattern.ToString(), RegexOptions.CultureInvariant | RegexOptions.Singleline, MatchTimeout);

        return new TranslationTemplate(regex, translation, culture, literalLength, !key.StartsWith('{'));
    }

    /// <summary>
    ///     Applies the template to the text if it matches.
    /// </summary>
    public bool TryApply(string text, Func<string, string> translate, [NotNullWhen(true)] out string? result)
    {
        var match = _pattern.Match(text);

        if (!match.Success)
        {
            result = null;
            return false;
        }

        result = TranslationPlaceholder().Replace(_translation, placeholder =>
        {
            var group = string.Create(CultureInfo.InvariantCulture, $"p{placeholder.Groups["index"].Value}");
            var value = match.Groups[group].Value;

            if (placeholder.Groups["forms"].Success)
            {
                return placeholder.Groups["forms"].Value.Split('|')[PluralRules.Select(_culture, value)];
            }

            return placeholder.Groups["translate"].Success ? translate(value) : value;
        });

        return true;
    }

    private static void EnsureTranslationPlaceholders(
        string key,
        string translation,
        HashSet<string> placeholders,
        CultureInfo culture)
    {
        foreach (Match placeholder in TranslationPlaceholder().Matches(translation))
        {
            if (!placeholders.Contains(placeholder.Groups["index"].Value))
            {
                throw new FormatException($"Translation of '{key}' uses placeholder {placeholder.Value} that is missing in the key.");
            }

            if (placeholder.Groups["forms"].Success
                && placeholder.Groups["forms"].Value.Split('|').Length != PluralRules.FormCountOf(culture))
            {
                throw new FormatException(
                    $"Translation of '{key}' must list {PluralRules.FormCountOf(culture)} plural forms in {placeholder.Value}.");
            }
        }

        if (AnyPlaceholder().IsMatch(TranslationPlaceholder().Replace(translation, string.Empty)))
        {
            throw new FormatException($"Translation of '{key}' contains a malformed placeholder.");
        }
    }

    [GeneratedRegex(@"\{(\d+)\}", RegexOptions.CultureInvariant)]
    private static partial Regex KeyPlaceholder();

    [GeneratedRegex(@"\{(?<index>\d+)(?::(?:(?<translate>t)|plural:(?<forms>[^{}|]+(?:\|[^{}|]+)+)))?\}", RegexOptions.CultureInvariant)]
    private static partial Regex TranslationPlaceholder();

    [GeneratedRegex(@"\{\d+[^{}]*\}", RegexOptions.CultureInvariant)]
    private static partial Regex AnyPlaceholder();
}