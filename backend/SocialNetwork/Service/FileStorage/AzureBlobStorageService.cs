using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using SocialNetwork.Helpers;

namespace SocialNetwork.Service;

public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobContainerClient _blobContainerClient;

    public AzureBlobStorageService(IOptions<AzureBlobStorageOptions> options)
    {
        var storageOptions = options.Value;

        if (string.IsNullOrWhiteSpace(storageOptions.ConnectionString))
        {
            throw new InvalidOperationException("Azure Blob connection string is not configured.");
        }

        if (string.IsNullOrWhiteSpace(storageOptions.ContainerName))
        {
            throw new InvalidOperationException("Azure Blob container name is not configured.");
        }

        _blobContainerClient = new BlobContainerClient(
            storageOptions.ConnectionString,
            storageOptions.ContainerName);
    }

    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken ct = default)
    {
        if (fileStream is null)
        {
            throw new ArgumentNullException(nameof(fileStream));
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("File name is required.", nameof(fileName));
        }

        await _blobContainerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }

        var blobName = BlobFileNameHelper.CreateUniqueBlobName(fileName);
        var blobClient = _blobContainerClient.GetBlobClient(blobName);

        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = string.IsNullOrWhiteSpace(contentType)
                    ? "application/octet-stream"
                    : contentType
            }
        };

        await blobClient.UploadAsync(fileStream, uploadOptions, ct);
        return blobClient.Uri.ToString();
    }

    public async Task<bool> DeleteAsync(string blobName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            return false;
        }

        var blobClient = _blobContainerClient.GetBlobClient(blobName);
        var response = await blobClient.DeleteIfExistsAsync(cancellationToken: ct);

        return response.Value;
    }
}
