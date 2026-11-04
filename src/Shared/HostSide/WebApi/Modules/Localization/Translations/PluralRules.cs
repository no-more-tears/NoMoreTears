using System.Globalization;

namespace NoMoreTears.Shared.HostSide.WebApi.Modules.Localization.Translations;

/// <summary>
///     Selects the plural form of a word for a number.
///     Ukrainian uses three forms (1 символ, 2 символи, 5 символів); other languages use two (1 item, 2 items).
/// </summary>
internal static class PluralRules
{
    private const string Ukrainian = "uk";

    /// <summary>
    ///     Returns how many plural forms a translation into the culture must list.
    /// </summary>
    public static int FormCountOf(CultureInfo culture) => IsUkrainian(culture) ? 3 : 2;

    /// <summary>
    ///     Returns the index of the form for the number written in the text.
    ///     Text that is not a number gets the last form.
    /// </summary>
    public static int Select(CultureInfo culture, string value)
    {
        var lastForm = FormCountOf(culture) - 1;

        if (!decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var number))
        {
            return lastForm;
        }

        number = Math.Abs(number);

        if (number != decimal.Truncate(number))
        {
            // Fractions: 1,5 секунди; 1.5 seconds.
            return IsUkrainian(culture) ? 1 : lastForm;
        }

        if (!IsUkrainian(culture))
        {
            return number == 1 ? 0 : 1;
        }

        var lastDigit = number % 10;
        var lastTwoDigits = number % 100;

        if (lastDigit == 1 && lastTwoDigits != 11)
        {
            return 0;
        }

        return lastDigit is >= 2 and <= 4 && lastTwoDigits is < 12 or > 14 ? 1 : 2;
    }

    private static bool IsUkrainian(CultureInfo culture)
    {
        return string.Equals(culture.TwoLetterISOLanguageName, Ukrainian, StringComparison.OrdinalIgnoreCase);
    }
}