namespace NoMoreTears.Modules.Requests.Contracts.Requests;

/// <summary>
///     Creates a new draft with the address and category of an earlier request and a new description.
/// </summary>
public sealed record DuplicateRequestRequest(
    string Description);