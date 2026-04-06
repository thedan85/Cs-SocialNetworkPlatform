namespace SocialNetwork.Service.Blob;

public sealed class BlobStorageOptions
{
	public const string SectionName = "AzureBlob";

	public string ConnectionString { get; set; } = string.Empty;

	public string ContainerName { get; set; } = "social-images";

	public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
}
