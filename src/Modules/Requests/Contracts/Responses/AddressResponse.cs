namespace NoMoreTears.Modules.Requests.Contracts.Responses;

public sealed record AddressResponse(
    string Value,
    CoordinatesResponse Coordinates);