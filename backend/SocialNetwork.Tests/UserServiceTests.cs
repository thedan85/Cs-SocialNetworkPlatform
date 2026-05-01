using Microsoft.AspNetCore.Identity;
using Moq;
using SocialNetwork.Dtos;
using SocialNetwork.Model;
using SocialNetwork.Repository;
using SocialNetwork.Service;
using Xunit;

namespace SocialNetwork.Tests.Services;

public class UsersServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPostRepository> _postRepoMock;
    private readonly Mock<IFriendshipRepository> _friendshipRepoMock;
    private readonly Mock<ILikeRepository> _likeRepoMock;
    private readonly Mock<IPostShareRepository> _postShareRepoMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly UsersService _usersService;

    public UsersServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _postRepoMock = new Mock<IPostRepository>();
        _friendshipRepoMock = new Mock<IFriendshipRepository>();
        _likeRepoMock = new Mock<ILikeRepository>();
        _postShareRepoMock = new Mock<IPostShareRepository>();

        _likeRepoMock
            .Setup(r => r.GetLikedPostIdsAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());
        _postShareRepoMock
            .Setup(r => r.GetSharedPostIdsAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());
        _postShareRepoMock
            .Setup(r => r.GetShareCountsAsync(
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<string, int>());
        
        // Mock UserManager cần một vài tham số giả lập để khởi tạo
        _userManagerMock = new Mock<UserManager<User>>(
            new Mock<IUserStore<User>>().Object, 
            null!, null!, null!, null!, null!, null!, null!, null!);

        _usersService = new UsersService(
            _userManagerMock.Object,
            _userRepoMock.Object,
            _postRepoMock.Object,
            _friendshipRepoMock.Object,
            _likeRepoMock.Object,
            _postShareRepoMock.Object);
    }

    #region GetUserByIdAsync Tests
    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists() 
    {
        // Arrange
        var userId = "user-123";
        var user = new User { Id = userId, UserName = "long_it" };
        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(user);

        // Act
        var result = await _usersService.GetUserByIdAsync(userId, userId, false);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(userId, result.Data!.UserId);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((User?)null);

        // Act
        var result = await _usersService.GetUserByIdAsync("actor-1", "wrong-id", false);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldHideEmail_WhenNotOwnerOrAdmin()
    {
        var userId = "user-1";
        var actorUserId = "actor-1";
        var user = new User { Id = userId, UserName = "user-1", Email = "user-1@example.com" };
        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(user);

        var result = await _usersService.GetUserByIdAsync(actorUserId, userId, false);

        Assert.True(result.Success);
        Assert.Equal(string.Empty, result.Data!.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldKeepEmail_WhenAdmin()
    {
        var userId = "user-1";
        var user = new User { Id = userId, UserName = "user-1", Email = "user-1@example.com" };
        _userRepoMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(user);

        var result = await _usersService.GetUserByIdAsync("admin-1", userId, true);

        Assert.True(result.Success);
        Assert.Equal("user-1@example.com", result.Data!.Email);
    }
    #endregion

    #region UpdateUserAsync Tests (Authorization & Logic)
    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateFields_WhenRequestIsValid()
    {
        // Arrange
        var userId = "user-1";
        var user = new User { Id = userId, Bio = "Old Bio" };
        var request = new UserUpdateRequest { Bio = "New Bio", IsActive = true };

        _userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                        .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _usersService.UpdateUserAsync(userId, request);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("New Bio", user.Bio);
        _userManagerMock.Verify(m => m.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnFailure_WhenIdentityUpdateFails()
    {
        // Arrange
        var userId = "user-1";
        var user = new User { Id = userId };
        _userManagerMock.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<User>()))
                        .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

        // Act
        var result = await _usersService.UpdateUserAsync(userId, new UserUpdateRequest());

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
    }
    #endregion

    #region GetUserPostsAsync Tests (Edge Cases)
    [Fact]
    public async Task GetUserPostsAsync_ShouldReturnPosts_WhenUserExists()
    {
        // Arrange
        var userId = "user-1";
        _userRepoMock.Setup(r => r.ExistsByIdAsync(userId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(true);
        _postRepoMock.Setup(r => r.GetByUserIdOrderedAsync(userId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<Post> { new Post { UserId = "p1" } });

        // Act
        var result = await _usersService.GetUserPostsAsync(userId, userId, false);

        // Assert
        Assert.True(result.Success);
        Assert.Single(result.Data!);
    }
    #endregion
}