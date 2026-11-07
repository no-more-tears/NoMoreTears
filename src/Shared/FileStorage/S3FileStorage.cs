using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using NoMoreTears.Shared.FileStorage.Abstractions;
using NoMoreTears.Shared.FileStorage.Exceptions;
using System.Collections.Concurrent;

namespace NoMoreTears.Shared.FileStorage;

public sealed class S3FileStorage(
    IMinioClient minioClient,
    ILogger<S3FileStorage> logger) : IFileStorage
{
    private static readonly ConcurrentDictionary<string, byte> KnownBuckets = new();

    public async Task UploadFileAsync(
        Stream payload,
        string bucketName,
        string fileName,
        string contentType,
        CancellationToken cancellationToken)
    {
        if (!payload.CanSeek)
        {
            throw new ArgumentException(
                "Payload stream must be seekable to determine its size.",
                nameof(payload));
        }

        try
        {
            await EnsureBucketExistsAsync(bucketName, cancellationToken);

            var size = payload.Length - payload.Position;

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithStreamData(payload)
                .WithObjectSize(size)
                .WithContentType(contentType);

            await minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            logger.LogInformation(
                "File '{FileName}' uploaded to bucket '{BucketName}'",
                fileName, bucketName);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (FileStorageException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FileStorageException(
                $"Failed to upload file '{fileName}' to bucket '{bucketName}'.", ex);
        }
    }

    public async Task<MemoryStream> DownloadFileAsync(
        string bucketName,
        string fileName,
        CancellationToken cancellationToken)
    {
        var memoryStream = new MemoryStream();

        try
        {
            var getObjectArgs = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(fileName)
                .WithCallbackStream(async (stream, ct) =>
                    await stream.CopyToAsync(memoryStream, ct));

            await minioClient.GetObjectAsync(getObjectArgs, cancellationToken);
            memoryStream.Position = 0;

            logger.LogInformation(
                "File '{FileName}' downloaded from bucket '{BucketName}'",
                fileName, bucketName);

            return memoryStream;
        }
        catch (Exception ex)
        {
            await memoryStream.DisposeAsync();

            switch (ex)
            {
                case OperationCanceledException:
                    throw;
                case ObjectNotFoundException or BucketNotFoundException:
                    throw new FileNotFoundInStorageException(bucketName, fileName, ex);
                default:
                    throw new FileStorageException(
                        $"Failed to download file '{fileName}' from bucket '{bucketName}'.", ex);
            }
        }
    }

    private async Task EnsureBucketExistsAsync(
        string bucketName,
        CancellationToken cancellationToken)
    {
        if (KnownBuckets.ContainsKey(bucketName))
        {
            return;
        }

        try
        {
            var existsArgs = new BucketExistsArgs().WithBucket(bucketName);

            if (!await minioClient.BucketExistsAsync(existsArgs, cancellationToken))
            {
                try
                {
                    var makeArgs = new MakeBucketArgs().WithBucket(bucketName);
                    await minioClient.MakeBucketAsync(makeArgs, cancellationToken);

                    logger.LogInformation("Bucket '{BucketName}' created", bucketName);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    var existsNow = await minioClient.BucketExistsAsync(existsArgs, cancellationToken);

                    if (!existsNow)
                    {
                        throw;
                    }
                }
            }

            KnownBuckets.TryAdd(bucketName, 0);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new FileStorageException(
                $"Failed to ensure bucket '{bucketName}' exists.", ex);
        }
    }
}