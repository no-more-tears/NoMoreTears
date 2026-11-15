namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Jobs;

public static class Routes
{
    public const string Base = "/requests/jobs";

    public const string Job = $"{Base}/{{requestId:guid}}";

    public const string History = $"{Job}/history";

    public const string MediaFile = $"{Job}/media/{{mediaFileId:guid}}";

    public const string Accept = $"{Job}/accept";

    public const string Done = $"{Job}/mark-as-done";

    public const string Refuse = $"{Job}/refuse";
}