namespace NoMoreTears.Shared.FileStorage.Exceptions;

public sealed class FileNotFoundInStorageException : FileStorageException
{
    public string BucketName { get; }

    public string FileName { get; }

    public FileNotFoundInStorageException(string bucketName, string fileName, Exception innerException)
        : base($"File '{fileName}' was not found in bucket '{bucketName}'.", innerException)
    {
        BucketName = bucketName;
        FileName = fileName;
    }
}