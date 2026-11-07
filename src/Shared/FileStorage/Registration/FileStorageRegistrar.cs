using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using NoMoreTears.Shared.FileStorage.Abstractions;

namespace NoMoreTears.Shared.FileStorage.Registration;

public static class FileStorageRegistrar
{
    public static IServiceCollection AddFileStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration
            .GetSection(FileStorageOptions.SectionName)
            .Get<FileStorageOptions>()
            ?? throw new InvalidOperationException(
                $"Section '{FileStorageOptions.SectionName}' is not configured.");

        services
            .AddSingleton(_ => new MinioClient()
                .WithEndpoint(options.Endpoint)
                .WithCredentials(options.AccessKey, options.SecretKey)
                .WithSSL(options.UseSsl)
                .Build())
            .AddSingleton<IFileStorage, S3FileStorage>();

        return services;
    }
}