using Microsoft.AspNetCore.Http;

namespace SocialNetwork.Service.Blob;

public interface IBlobService
{
	Task<string> UploadImageAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);

	Task<bool> DeleteImageByUrlAsync(string blobUrl, CancellationToken cancellationToken = default);
}
