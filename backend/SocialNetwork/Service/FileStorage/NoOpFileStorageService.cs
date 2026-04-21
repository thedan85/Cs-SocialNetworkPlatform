namespace SocialNetwork.Service;

/// <summary>
/// No-op implementation of IFileStorageService for development environments
/// where Azure Blob Storage is not configured.
/// </summary>
public class NoOpFileStorageService : IFileStorageService
{
    public Task<string> UploadAsync(
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

        // Return a dummy URL for development
        var dummyUrl = $"https://localhost/files/{fileName}";
        return Task.FromResult(dummyUrl);
    }

    public Task<bool> DeleteAsync(string blobName, CancellationToken ct = default)
    {
        // No-op: pretend deletion succeeded
        return Task.FromResult(true);
    }
}
