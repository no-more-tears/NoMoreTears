using System.Globalization;
using NoMoreTears.Shared.Providers.Identity.Contracts.UserActions;

namespace NoMoreTears.Modules.Identity.Applications.Services.Configuration;

/// <summary>
///     Builds client links from the URL templates of <see cref="IdentityLinkOptions"/>.
/// </summary>
public static class IdentityLinks
{
    public const string UserIdPlaceholder = "{userId}";
    public const string TokenPlaceholder = "{token}";

    /// <summary>
    ///     Builds a link.
    /// </summary>
    public static string Build(
        string urlTemplate,
        UserActionTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(urlTemplate);
        ArgumentNullException.ThrowIfNull(ticket);

        return urlTemplate
            .Replace(UserIdPlaceholder, ticket.UserId.ToString("D", CultureInfo.InvariantCulture), StringComparison.Ordinal)
            .Replace(TokenPlaceholder, Uri.EscapeDataString(ticket.Token), StringComparison.Ordinal);
    }
}