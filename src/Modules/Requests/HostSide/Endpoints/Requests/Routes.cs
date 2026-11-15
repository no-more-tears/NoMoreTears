namespace NoMoreTears.Modules.Requests.HostSide.Endpoints.Requests;

public static class Routes
{
    public const string Base = "/requests";

    public const string Request = $"{Base}/{{requestId:guid}}";

    public const string Category = $"{Request}/category";

    public const string Address = $"{Request}/address";

    public const string Description = $"{Request}/description";

    public const string History = $"{Request}/history";

    public const string Media = $"{Request}/media";

    public const string MediaFile = $"{Media}/{{mediaFileId:guid}}";

    public const string Upload = $"{Request}/uploads/{{uploadId:guid}}";

    public const string Publish = $"{Request}/publish";

    public const string Extend = $"{Request}/extend";

    public const string Republish = $"{Request}/republish";

    public const string Cancel = $"{Request}/cancel";

    public const string ConfirmCompletion = $"{Request}/confirm-completion";

    public const string ObjectToCompletion = $"{Request}/object-to-completion";

    public const string ReportNoShow = $"{Request}/report-no-show";

    public const string Duplicate = $"{Request}/duplicate";
}