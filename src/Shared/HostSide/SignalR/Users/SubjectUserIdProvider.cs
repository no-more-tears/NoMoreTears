using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace NoMoreTears.Shared.HostSide.SignalR.Users;

/// <summary>
///     Identifies the user of a connection by the subject of the access token,
///     so that <c>Clients.User(userId)</c> reaches every connection of the user on every instance.
/// </summary>
internal sealed class SubjectUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        ArgumentNullException.ThrowIfNull(connection);

        var user = connection.User;

        return user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}