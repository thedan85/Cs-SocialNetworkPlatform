using Microsoft.Extensions.Options;
using SocialNetwork.Service;
using Xunit;

namespace SocialNetwork.Tests.Services;

public class FileStorageServiceTests
{
    [Fact]
    public async Task NoOpFileStorageService_UploadAsync_ShouldReturnDummyUrl()
    {
        var service = new NoOpFileStorageService();
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        var url = await service.UploadAsync(stream, "file.txt", "text/plain");

        Assert.Equal("https://localhost/files/file.txt", url);
    }

    [Fact]
    public async Task NoOpFileStorageService_UploadAsync_ShouldThrow_WhenStreamNull()
    {
        var service = new NoOpFileStorageService();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.UploadAsync(null!, "file.txt", "text/plain"));
    }

    [Fact]
    public async Task NoOpFileStorageService_UploadAsync_ShouldThrow_WhenFileNameMissing()
    {
        var service = new NoOpFileStorageService();
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UploadAsync(stream, "", "text/plain"));
    }

    [Fact]
    public async Task NoOpFileStorageService_DeleteAsync_ShouldReturnTrue()
    {
        var service = new NoOpFileStorageService();

        var result = await service.DeleteAsync("any");

        Assert.True(result);
    }

    [Fact]
    public void AzureBlobStorageService_ShouldThrow_WhenConnectionStringMissing()
    {
        var options = Options.Create(new AzureBlobStorageOptions
        {
            ConnectionString = "",
            ContainerName = "container"
        });

        Assert.Throws<InvalidOperationException>(() => new AzureBlobStorageService(options));
    }

    [Fact]
    public void AzureBlobStorageService_ShouldThrow_WhenContainerNameMissing()
    {
        var options = Options.Create(new AzureBlobStorageOptions
        {
            ConnectionString = "UseDevelopmentStorage=true",
            ContainerName = ""
        });

        Assert.Throws<InvalidOperationException>(() => new AzureBlobStorageService(options));
    }

    [Fact]
    public async Task AzureBlobStorageService_UploadAsync_ShouldThrow_WhenStreamNull()
    {
        var service = CreateAzureService();

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            service.UploadAsync(null!, "file.txt", "text/plain"));
    }

    [Fact]
    public async Task AzureBlobStorageService_UploadAsync_ShouldThrow_WhenFileNameMissing()
    {
        var service = CreateAzureService();
        using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

        await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UploadAsync(stream, "", "text/plain"));
    }

    [Fact]
    public async Task AzureBlobStorageService_DeleteAsync_ShouldReturnFalse_WhenBlobNameMissing()
    {
        var service = CreateAzureService();

        var result = await service.DeleteAsync("");

        Assert.False(result);
    }

    [Fact]
    public void AzureBlobStorageOptions_ShouldHaveDefaults()
    {
        var options = new AzureBlobStorageOptions();

        Assert.Equal(string.Empty, options.ConnectionString);
        Assert.Equal(string.Empty, options.ContainerName);
        Assert.Equal("AzureBlobStorage", AzureBlobStorageOptions.SectionName);
    }

    [Fact]
    public void SharePostResult_ShouldInitializeDefaults()
    {
        var result = new SharePostResult();

        Assert.NotNull(result.Share);
        Assert.False(result.IsCreated);
    }

    private static AzureBlobStorageService CreateAzureService()
    {
        var options = Options.Create(new AzureBlobStorageOptions
        {
            ConnectionString = "UseDevelopmentStorage=true",
            ContainerName = "test"
        });

        return new AzureBlobStorageService(options);
    }
}
