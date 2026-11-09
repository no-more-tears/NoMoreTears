using System.Globalization;

namespace NoMoreTears.Modules.Notification.Applications.Services.Emails;

/// <summary>
///     Builds the placeholder values of the email templates listed in <see cref="EmailTemplates"/>.
/// </summary>
public static class EmailModels
{
    public static IReadOnlyDictionary<string, string> WithLink(
        string email,
        string link,
        DateTimeOffset expiresAt)
    {
        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Email"] = email,
            ["Link"] = link,
            ["ExpiresAt"] = FormatMoment(expiresAt)
        };
    }

    public static IReadOnlyDictionary<string, string> WithCode(
        string email,
        string code,
        DateTimeOffset expiresAt)
    {
        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["Email"] = email,
            ["Code"] = code,
            ["ExpiresAt"] = FormatMoment(expiresAt)
        };
    }

    /// <summary>
    ///     Formats a moment for texts read by people, independent of the server culture.
    /// </summary>
    public static string FormatMoment(DateTimeOffset moment) =>
        moment.UtcDateTime.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture);
}