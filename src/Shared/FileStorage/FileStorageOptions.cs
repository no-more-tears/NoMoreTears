namespace NoMoreTears.Shared.FileStorage;

public sealed class FileStorageOptions
{
    public const string SectionName = "FileStorageSettings";

    public string Endpoint { get; set; } = "localhost:9000";

    public string AccessKey { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public bool UseSsl { get; set; }
}