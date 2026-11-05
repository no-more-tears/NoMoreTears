using System.Globalization;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Localization.Translations;

/// <summary>
///     Translates English response messages into the language requested by the client.
/// </summary>
public interface IMessageTranslator
{
    /// <summary>
    ///     Translates the text into the current UI culture of the request.
    ///     Returns the text unchanged when no translation is known.
    /// </summary>
    string Translate(string text);

    /// <summary>
    ///     Translates the text into the given culture.
    ///     Returns the text unchanged when no translation is known.
    /// </summary>
    string Translate(string text, CultureInfo culture);
}