namespace SocialNetwork.Service;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default);
    Task<bool> DeleteAsync(string blobName, CancellationToken ct = default);
}
