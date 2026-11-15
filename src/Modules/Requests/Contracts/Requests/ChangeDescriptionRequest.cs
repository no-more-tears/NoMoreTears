namespace NoMoreTears.Modules.Requests.Contracts.Requests;

/// <summary>
///     Replaces the description of a draft.
/// </summary>
public sealed record ChangeDescriptionRequest(
    string Description);