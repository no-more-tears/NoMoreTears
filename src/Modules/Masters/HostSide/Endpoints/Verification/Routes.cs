namespace NoMoreTears.Modules.Masters.HostSide.Endpoints.Verification;

public static class Routes
{
    public const string Masters = "/masters";

    public const string Master = $"{Masters}/{{masterId:guid}}";

    public const string Document = $"{Master}/documents/{{documentId:guid}}";

    public const string VerifyCategory = $"{Master}/categories/{{categoryId:guid}}/verify";

    public const string RejectCategory = $"{Master}/categories/{{categoryId:guid}}/reject";

    public const string VerifyServiceArea = $"{Master}/service-area/verify";

    public const string RejectServiceArea = $"{Master}/service-area/reject";

    public const string Verify = $"{Master}/verify";

    public const string Reject = $"{Master}/reject";

    public const string Hold = $"{Master}/verification-hold";
}
