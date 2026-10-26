using System.Net;
using System.Text.RegularExpressions;

namespace NoMoreTears.Modules.Notification.Applications.Services.Templates;

/// <summary>
///     Substitutes <c>{{Key}}</c> placeholders of a template with model values.
/// </summary>
/// <remarks>
///     Values are HTML-encoded in the HTML body. A placeholder without a model value is a
///     programming error and fails the rendering instead of sending a broken email.
/// </remarks>
public static partial class EmailTemplateRenderer
{
    public static RenderedEmail Render(
        EmailTemplate template,
        IReadOnlyDictionary<string, string> model)
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(model);

        return new RenderedEmail(
            Substitute(template.Subject, model, encode: false),
            Substitute(template.HtmlBody, model, encode: true),
            Substitute(template.TextBody, model, encode: false));
    }

    private static string Substitute(
        string text,
        IReadOnlyDictionary<string, string> model,
        bool encode)
    {
        return Placeholder().Replace(text, match =>
        {
            var key = match.Groups["key"].Value;
            if (!model.TryGetValue(key, out var value))
            {
                throw new InvalidOperationException($"Email template placeholder '{key}' has no model value.");
            }

            return encode ? WebUtility.HtmlEncode(value) : value;
        });
    }

    [GeneratedRegex(@"\{\{\s*(?<key>\w+)\s*\}\}", RegexOptions.CultureInvariant)]
    private static partial Regex Placeholder();
}
