using NoMoreTears.Modules.Requests.Contracts.Requests;
using NoMoreTears.Modules.Requests.Domain.Exceptions;
using NoMoreTears.Modules.Requests.Domain.ValueObjects;

namespace NoMoreTears.Modules.Requests.Applications.Services.Mappings;

/// <summary>
///     Manual mapping of requests to value objects; the factories of the value objects enforce the domain rules.
/// </summary>
public static class RequestMappings
{
    public static Address ToAddress(this AddressRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Coordinates is null)
        {
            throw new InvalidAddressException("Coordinates of the address are required.");
        }

        return Address.Of(
            request.Value,
            Coordinates.Of(request.Coordinates.Latitude, request.Coordinates.Longitude));
    }

    /// <summary>
    ///     Returns the reason.
    /// </summary>
    public static Reason? ToOptionalReason(this string? reason) =>
        string.IsNullOrWhiteSpace(reason) ? null : Reason.Of(reason);
}