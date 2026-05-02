using Moq;
using SocialNetwork.Model;
using SocialNetwork.Repository;
using SocialNetwork.Service;
using Xunit;

namespace SocialNetwork.Tests.Services;

public class HashtagsServiceTests
{
    private readonly Mock<IHashtagRepository> _hashtagRepoMock;
    private readonly HashtagsService _hashtagsService;

    public HashtagsServiceTests()
    {
        _hashtagRepoMock = new Mock<IHashtagRepository>();
        _hashtagsService = new HashtagsService(_hashtagRepoMock.Object);
    }

    [Fact]
    public async Task SearchHashtagsAsync_ShouldReturnEmpty_WhenQueryBlank()
    {
        var result = await _hashtagsService.SearchHashtagsAsync("  ");

        Assert.True(result.Success);
        Assert.Empty(result.Data!);
        _hashtagRepoMock.Verify(
            r => r.SearchAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SearchHashtagsAsync_ShouldReturnMappedPosts_WhenResultsExist()
    {
        var older = new Post { PostId = "p1", Content = "Old", CreatedAt = new DateTime(2026, 5, 1) };
        var newer = new Post { PostId = "p2", Content = "New", CreatedAt = new DateTime(2026, 5, 2) };
        var hashtag = new Hashtag
        {
            Tag = "test",
            UsageCount = 3,
            PostHashtags = new List<PostHashtag>
            {
                new PostHashtag { Post = older },
                new PostHashtag { Post = newer }
            }
        };

        _hashtagRepoMock
            .Setup(r => r.SearchAsync("test", 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Hashtag> { hashtag });

        var result = await _hashtagsService.SearchHashtagsAsync("test", 1, 10, postsPerHashtag: 1);

        Assert.True(result.Success);
        Assert.Single(result.Data!);
        Assert.Single(result.Data![0].Posts);
        Assert.Equal("p2", result.Data![0].Posts[0].PostId);
    }

    [Fact]
    public async Task GetTrendingHashtagsAsync_ShouldReturnResults()
    {
        _hashtagRepoMock
            .Setup(r => r.GetTrendingAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Hashtag>
            {
                new Hashtag { Tag = "trend-1", UsageCount = 10 },
                new Hashtag { Tag = "trend-2", UsageCount = 5 }
            });

        var result = await _hashtagsService.GetTrendingHashtagsAsync();

        Assert.True(result.Success);
        Assert.Equal(2, result.Data!.Count);
        Assert.Equal("trend-1", result.Data[0].Tag);
    }
}
