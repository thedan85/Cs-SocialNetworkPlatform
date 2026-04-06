using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace SocialNetwork.Service.Blob;

public sealed class BlobService : IBlobService
{
	private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
	{
		".jpg", ".jpeg", ".png", ".webp", ".gif"
	};

	private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
	{
		"image/jpeg", "image/png", "image/webp", "image/gif"
	};

	private readonly BlobContainerClient _containerClient;
	private readonly BlobStorageOptions _options;

	public BlobService(IOptions<BlobStorageOptions> options)
	{
		_options = options.Value;

		if (string.IsNullOrWhiteSpace(_options.ConnectionString))
		{
			throw new InvalidOperationException("AzureBlob:ConnectionString is missing.");
		}

		if (string.IsNullOrWhiteSpace(_options.ContainerName))
		{
			throw new InvalidOperationException("AzureBlob:ContainerName is missing.");
		}

		var serviceClient = new BlobServiceClient(_options.ConnectionString);
		_containerClient = serviceClient.GetBlobContainerClient(_options.ContainerName.Trim());
	}

	public async Task<string> UploadImageAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
	{
		ValidateFile(file);

		var normalizedFolder = NormalizeFolder(folder);
		var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
		var blobName = $"{normalizedFolder}/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{extension}";

		await _containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

		var blobClient = _containerClient.GetBlobClient(blobName);
		await using var stream = file.OpenReadStream();
		var uploadOptions = new BlobUploadOptions
		{
			HttpHeaders = new BlobHttpHeaders
			{
				ContentType = file.ContentType
			},
			Metadata = new Dictionary<string, string>
			{
				["uploadedAtUtc"] = DateTime.UtcNow.ToString("O")
			}
		};

		await blobClient.UploadAsync(stream, uploadOptions, cancellationToken);
		return blobClient.Uri.ToString();
	}

	public async Task<bool> DeleteImageByUrlAsync(string blobUrl, CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(blobUrl))
		{
			return false;
		}

		if (!Uri.TryCreate(blobUrl, UriKind.Absolute, out var blobUri))
		{
			return false;
		}

		var blobName = TryGetBlobName(blobUri);
		if (string.IsNullOrWhiteSpace(blobName))
		{
			return false;
		}

		var blobClient = _containerClient.GetBlobClient(blobName);
		var result = await blobClient.DeleteIfExistsAsync(
			DeleteSnapshotsOption.IncludeSnapshots,
			cancellationToken: cancellationToken);

		return result.Value;
	}

	private void ValidateFile(IFormFile file)
	{
		if (file is null || file.Length == 0)
		{
			throw new ArgumentException("File is empty.", nameof(file));
		}

		if (file.Length > _options.MaxFileSizeBytes)
		{
			throw new ArgumentException(
				$"File size exceeds the limit of {_options.MaxFileSizeBytes} bytes.",
				nameof(file));
		}

		var extension = Path.GetExtension(file.FileName);
		if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
		{
			throw new ArgumentException("Unsupported file extension.", nameof(file));
		}

		if (string.IsNullOrWhiteSpace(file.ContentType) || !AllowedContentTypes.Contains(file.ContentType))
		{
			throw new ArgumentException("Unsupported content type.", nameof(file));
		}
	}

	private string? TryGetBlobName(Uri blobUri)
	{
		var decodedPath = Uri.UnescapeDataString(blobUri.AbsolutePath.TrimStart('/'));
		var expectedPrefix = _containerClient.Name + "/";

		if (!decodedPath.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}

		return decodedPath[expectedPrefix.Length..];
	}

	private static string NormalizeFolder(string folder)
	{
		if (string.IsNullOrWhiteSpace(folder))
		{
			return "misc";
		}

		return folder.Replace('\\', '/').Trim('/').ToLowerInvariant();
	}
}
