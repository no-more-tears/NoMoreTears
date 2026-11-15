namespace NoMoreTears.Modules.Requests.Contracts.Requests;

/// <summary>
///     Cancels a request; the reason is optional before acceptance and required after it.
/// </summary>
public sealed record CancelRequestRequest(
    string? Reason);