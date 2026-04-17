using Moq;
using SocialNetwork.Dtos;
using SocialNetwork.Model;
using SocialNetwork.Repository;
using SocialNetwork.Service;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SocialNetwork.Tests.Services;

public class StoriesServiceTests
{
    private readonly Mock<IStoryRepository> _storyRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly StoriesService _storiesService;

    public StoriesServiceTests()
    {
        _storyRepoMock = new Mock<IStoryRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _storiesService = new StoriesService(_storyRepoMock.Object, _userRepoMock.Object);
    }

    // --- 1. GET STORIES ---

    [Fact]
    public async Task GetStoriesAsync_ShouldReturnOk_WithData()
    {
        // Arrange
        _storyRepoMock.Setup(r => r.GetActiveAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new List<Story> { new Story { Content = "Story 1" } });

        // Act
        var result = await _storiesService.GetStoriesAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data!);
    }

    // --- 2. GET BY ID ---

    [Fact]
    public async Task GetStoryByIdAsync_ShouldReturnNotFound_WhenStoryDoesNotExist()
    {
        _storyRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync((Story?)null);

        var result = await _storiesService.GetStoryByIdAsync("non-existent");

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    // --- 3. GET FOR USER ---

    [Fact]
    public async Task GetStoriesForUserAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        _userRepoMock.Setup(r => r.ExistsByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(false);

        var result = await _storiesService.GetStoriesForUserAsync("user-1");

        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    // --- 4. CREATE STORY (Edge Case: Expiration) ---

    [Fact]
    public async Task CreateStoryAsync_ShouldReturnValidation_WhenExpirationIsInPast()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new User { Id = "user-1" });
        var request = new StoryCreateRequest 
        { 
            Content = "Old Story", 
            ExpiresAt = DateTime.UtcNow.AddMinutes(-10) // Thời gian trong quá khứ
        };

        // Act
        var result = await _storiesService.CreateStoryAsync("user-1", request);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
        Assert.Contains("future", result.Errors[0]);
    }

    [Fact]
    public async Task CreateStoryAsync_ShouldSucceed_WithValidData()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new User { Id = "user-1" });
        var request = new StoryCreateRequest { Content = "New Story" };

        var result = await _storiesService.CreateStoryAsync("user-1", request);

        Assert.True(result.Success);
        _storyRepoMock.Verify(r => r.AddAsync(It.IsAny<Story>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    // --- 5. DELETE STORY (Auth Logic) ---

    [Fact]
    public async Task DeleteStoryAsync_ShouldReturnUnauthorized_WhenUserIsNotOwnerAndNotAdmin()
    {
        // Arrange
        var storyId = "story-123";
        var story = new Story { UserId = "owner-id" };
        _storyRepoMock.Setup(r => r.GetByIdAsync(storyId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(story);

        // Act
        var result = await _storiesService.DeleteStoryAsync("other-user", storyId, false);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task DeleteStoryAsync_ShouldSucceed_WhenUserIsAdmin()
    {
        // Arrange
        var storyId = "story-123";
        var story = new Story { UserId = "owner-id" };
        _storyRepoMock.Setup(r => r.GetByIdAsync(storyId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(story);
        _storyRepoMock.Setup(r => r.DeleteAsync(storyId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(true);

        // Act
        var result = await _storiesService.DeleteStoryAsync("admin-id", storyId, true);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Story deleted.", result.Data);
    }

    [Fact]
    public async Task DeleteStoryAsync_ShouldReturnNotFound_WhenRepoDeleteReturnsFalse()
    {
        var storyId = "story-123";
        var story = new Story { UserId = "owner-id" };
        _storyRepoMock.Setup(r => r.GetByIdAsync(storyId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(story);
        // Giả sử tìm thấy nhưng khi thực hiện xóa thì Repo báo không xóa được (ví dụ đã bị xóa bởi thread khác)
        _storyRepoMock.Setup(r => r.DeleteAsync(storyId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(false);

        var result = await _storiesService.DeleteStoryAsync("owner-id", storyId, false);

        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    // --- BỔ SUNG ĐỂ ĐẠT MỤC TIÊU 15+ METHODS TRONG PROJECT ---
    [Fact] public async Task GetStoriesAsync_ShouldHandleEmptyList() {
        _storyRepoMock.Setup(r => r.GetActiveAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(new List<Story>());
        var result = await _storiesService.GetStoriesAsync();
        Assert.Empty(result.Data!);
    }

    [Fact] public async Task GetStoryByIdAsync_ShouldReturnData_WhenFound() {
        _storyRepoMock.Setup(r => r.GetByIdAsync("1", It.IsAny<CancellationToken>())).ReturnsAsync(new Story { Content = "S" });
        var result = await _storiesService.GetStoryByIdAsync("1");
        Assert.True(result.Success);
    }

    [Fact] public async Task DeleteStoryAsync_ShouldReturnUnauthorized_WhenActorIdEmpty() {
        var result = await _storiesService.DeleteStoryAsync("", "s1", false);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }
}