using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SocialNetwork.Helpers;

namespace SocialNetwork.Service;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _webRootPath;
    private readonly string _uploadsPath;
    private readonly string _relativeRoot;

    public LocalFileStorageService(IWebHostEnvironment environment, IOptions<LocalFileStorageOptions> options)
    {
        var storageOptions = options.Value ?? new LocalFileStorageOptions();

        var webRootPath = environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRootPath))
        {
            webRootPath = Path.Combine(environment.ContentRootPath, "wwwroot");
        }

        _webRootPath = webRootPath;

        var normalizedUploadsPath = NormalizeRelativePath(storageOptions.UploadsPath);
        _relativeRoot = "/" + normalizedUploadsPath;
        var uploadSegments = normalizedUploadsPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var combinedSegments = new string[uploadSegments.Length + 1];
        combinedSegments[0] = _webRootPath;
        for (var index = 0; index < uploadSegments.Length; index++)
        {
            combinedSegments[index + 1] = uploadSegments[index];
        }
        _uploadsPath = Path.Combine(combinedSegments);

        Directory.CreateDirectory(_uploadsPath);
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

        var safeFileName = BlobFileNameHelper.CreateUniqueBlobName(fileName);
        var destinationPath = Path.Combine(_uploadsPath, safeFileName);

        if (fileStream.CanSeek)
        {
            fileStream.Position = 0;
        }

        await using var output = new FileStream(
            destinationPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            81920,
            useAsync: true);

        await fileStream.CopyToAsync(output, ct);

        return $"{_relativeRoot}/{safeFileName}".Replace("\\", "/");
    }

    public Task<bool> DeleteAsync(string blobName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(blobName))
        {
            return Task.FromResult(false);
        }

        var relativePath = ExtractRelativePath(blobName);
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return Task.FromResult(false);
        }

        var targetPath = Path.Combine(_webRootPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
        var fullTargetPath = Path.GetFullPath(targetPath);
        var fullUploadsPath = Path.GetFullPath(_uploadsPath);

        if (!fullTargetPath.StartsWith(fullUploadsPath, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(false);
        }

        if (!File.Exists(fullTargetPath))
        {
            return Task.FromResult(false);
        }

        File.Delete(fullTargetPath);
        return Task.FromResult(true);
    }

    private static string NormalizeRelativePath(string? path)
    {
        var normalized = (path ?? string.Empty).Replace("\\", "/").Trim('/');
        return string.IsNullOrWhiteSpace(normalized) ? "uploads" : normalized;
    }

    private static string ExtractRelativePath(string path)
    {
        var candidate = path.Trim();

        if (Uri.TryCreate(candidate, UriKind.Absolute, out var absolute))
        {
            candidate = absolute.AbsolutePath;
        }

        return candidate.TrimStart('/', '\\');
    }
}
