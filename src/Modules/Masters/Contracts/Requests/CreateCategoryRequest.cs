namespace NoMoreTears.Modules.Masters.Contracts.Requests;

public sealed record CreateCategoryRequest(
    string Name,
    string? Description);
