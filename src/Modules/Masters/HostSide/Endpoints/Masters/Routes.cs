namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Masters;

public static class Routes
{
    public const string Base = "/masters";

    public const string Me = $"{Base}/me";

    public const string FullName = $"{Me}/full-name";

    public const string ServiceArea = $"{Me}/service-area";

    public const string WorkSchedule = $"{Me}/work-schedule";

    public const string Availability = $"{Me}/availability";

    public const string Categories = $"{Me}/categories";

    public const string Category = $"{Categories}/{{categoryId:guid}}";

    public const string Avatar = $"{Me}/avatar";

    public const string Documents = $"{Me}/documents";

    public const string Document = $"{Documents}/{{documentId:guid}}";

    public const string Upload = $"{Me}/uploads/{{uploadId:guid}}";

    public const string Submit = $"{Me}/submit";

    public const string MasterAvatar = $"{Base}/{{masterId:guid}}/avatar";
}
