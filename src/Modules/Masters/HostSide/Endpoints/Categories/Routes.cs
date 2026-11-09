namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Categories;

public static class Routes
{
    public const string Base = "/categories";

    public const string Category = $"{Base}/{{categoryId:guid}}";

    public const string Archive = $"{Category}/archive";

    public const string Activate = $"{Category}/activate";
}
