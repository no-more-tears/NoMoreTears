using System.Globalization;
using NoMoreTears.Modules.Notification.Applications.Services.Emails;

namespace NoMoreTears.Modules.Notification.HostSide.SignalR.Consumers;

/// <summary>
///     Builds the <c>Data</c> of a notification with culture-independent values; empty values are skipped.
/// </summary>
internal static class NotificationData
{
    public static IReadOnlyDictionary<string, string> Of(params (string Key, object? Value)[] entries)
    {
        var data = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var (key, value) in entries)
        {
            var text = value switch
            {
                null => null,
                Guid id => id.ToString("D", CultureInfo.InvariantCulture),
                DateTimeOffset moment => EmailModels.FormatMoment(moment),
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString()
            };

            if (!string.IsNullOrEmpty(text))
            {
                data[key] = text;
            }
        }

        return data;
    }
}