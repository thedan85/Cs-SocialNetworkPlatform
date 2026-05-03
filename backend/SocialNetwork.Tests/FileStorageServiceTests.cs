using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
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
    public async Task LocalFileStorageService_UploadAsync_ShouldPersistFileAndReturnRelativePath()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), $"sn-uploads-{Guid.NewGuid():N}");
        var webRootPath = Path.Combine(tempRoot, "wwwroot");
        Directory.CreateDirectory(webRootPath);

        try
        {
            var env = new TestWebHostEnvironment(tempRoot, webRootPath);
            var options = Options.Create(new LocalFileStorageOptions
            {
                UploadsPath = "uploads/images"
            });
            var service = new LocalFileStorageService(env, options);

            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });
            var relativePath = await service.UploadAsync(stream, "avatar.png", "image/png");

            Assert.StartsWith("/uploads/images/", relativePath);

            var fullPath = Path.Combine(
                webRootPath,
                relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            Assert.True(File.Exists(fullPath));

            var deleted = await service.DeleteAsync(relativePath);
            Assert.True(deleted);
            Assert.False(File.Exists(fullPath));
        }
        finally
        {
            if (Directory.Exists(tempRoot))
            {
                Directory.Delete(tempRoot, recursive: true);
            }
        }
    }

    [Fact]
    public void SharePostResult_ShouldInitializeDefaults()
    {
        var result = new SharePostResult();

        Assert.NotNull(result.Share);
        Assert.False(result.IsCreated);
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public TestWebHostEnvironment(string contentRootPath, string webRootPath)
        {
            ContentRootPath = contentRootPath;
            WebRootPath = webRootPath;
            EnvironmentName = Environments.Development;
            ApplicationName = "SocialNetwork.Tests";
            ContentRootFileProvider = new PhysicalFileProvider(ContentRootPath);
            WebRootFileProvider = new PhysicalFileProvider(WebRootPath);
        }

        public string ApplicationName { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string WebRootPath { get; set; }
        public string EnvironmentName { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
