namespace NoMoreTears.Modules.Masters.Contracts.Requests;

public sealed record UpdateCategoryRequest(
    string Name,
    string? Description);
