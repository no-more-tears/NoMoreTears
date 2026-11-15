namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Support;

public static class Routes
{
    public const string Base = "/requests/support";

    public const string Request = $"{Base}/{{requestId:guid}}";

    public const string History = $"{Request}/history";

    public const string MediaFile = $"{Request}/media/{{mediaFileId:guid}}";
}