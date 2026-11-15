namespace NoMoreTears.Modules.Requests.Contracts.Requests;

/// <summary>
///     Chooses another category of the catalog for a draft.
/// </summary>
public sealed record ChangeCategoryRequest(
    Guid CategoryId);