namespace NoMoreTears.Shared.FileStorage.Abstractions;

public interface IFileStorage
{
    Task UploadFileAsync(
        Stream payload,
        string bucketName,
        string fileName,
        string contentType,
        CancellationToken cancellationToken);

    Task<MemoryStream> DownloadFileAsync(
        string bucketName,
        string fileName,
        CancellationToken cancellationToken);
}