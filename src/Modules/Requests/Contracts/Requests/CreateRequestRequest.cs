namespace NoMoreTears.Modules.Requests.Contracts.Requests;

/// <summary>
///     Creates a draft request of the current client.
/// </summary>
public sealed record CreateRequestRequest(
    Guid CategoryId,
    AddressRequest Address,
    string Description);