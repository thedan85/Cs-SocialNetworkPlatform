using Moq;
using SocialNetwork.Dtos;
using SocialNetwork.Model;
using SocialNetwork.Repository;
using SocialNetwork.Service;
using Xunit;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Hubs;

namespace SocialNetwork.Tests.Services;

public class PostsServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPostRepository> _postRepoMock;
    private readonly Mock<ICommentRepository> _commentRepoMock;
    private readonly Mock<ILikeRepository> _likeRepoMock;
    private readonly Mock<IPostShareRepository> _postShareRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IHashtagRepository> _hashtagRepoMock;
    private readonly Mock<IFriendshipRepository> _friendshipRepoMock;
    private readonly Mock<INotificationRepository> _notificationRepoMock;
    private readonly Mock<IHubContext<NotificationsHub>> _notificationHubMock;
    private readonly Mock<IPostReportRepository> _reportRepoMock;
    private readonly PostsService _postsService;

    public PostsServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _postRepoMock = new Mock<IPostRepository>();
        _commentRepoMock = new Mock<ICommentRepository>();
        _likeRepoMock = new Mock<ILikeRepository>();
        _postShareRepoMock = new Mock<IPostShareRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _hashtagRepoMock = new Mock<IHashtagRepository>();
        _friendshipRepoMock = new Mock<IFriendshipRepository>();
        _notificationRepoMock = new Mock<INotificationRepository>();
        _notificationHubMock = new Mock<IHubContext<NotificationsHub>>();
        _reportRepoMock = new Mock<IPostReportRepository>();

        _likeRepoMock
            .Setup(r => r.GetLikedPostIdsAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());
        _likeRepoMock
            .Setup(r => r.GetByPostAndUserAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Like?)null);
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
        _postShareRepoMock
            .Setup(r => r.GetByPostAndUserAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((PostShare?)null);
        _postShareRepoMock
            .Setup(r => r.CountByPostIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        _postsService = new PostsService(
            _unitOfWorkMock.Object,
            _postRepoMock.Object,
            _commentRepoMock.Object,
            _likeRepoMock.Object,
            _postShareRepoMock.Object,
            _userRepoMock.Object,
            _hashtagRepoMock.Object,
                _friendshipRepoMock.Object,
            _notificationRepoMock.Object,
            _notificationHubMock.Object,
            _reportRepoMock.Object);
    }

    // --- 1. GET POST BY ID ---
    #region  GET POST BY ID
    [Fact]
    public async Task GetPostByIdAsync_ShouldReturnOk_WhenPostExists()
    {
        // Arrange
        var postId = "post-123";
        var actorUserId = "user-1";
        var post = new Post { Content = "Hello world", UserId = actorUserId };
        _postRepoMock.Setup(r => r.GetByIdAsync(postId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(post);

        // Act
        var result = await _postsService.GetPostByIdAsync(actorUserId, postId, false);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal("Hello world", result.Data.Content);
    }

    [Fact]
    public async Task GetPostByIdAsync_ShouldReturnNotFound_WhenPostDoesNotExist()
    {
        // Arrange
        _postRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Post?)null);

        // Act
        var result = await _postsService.GetPostByIdAsync("user-1", "invalid-id", false);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    [Fact] 
    public async Task GetPostsAsync_ShouldReturnSuccess() { 
        _postRepoMock.Setup(r => r.GetVisiblePagedAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<Post>());
        var result = await _postsService.GetPostsAsync("user-1", false);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task GetPostsAsync_ShouldReturnUnauthorized_WhenActorMissing()
    {
        var result = await _postsService.GetPostsAsync("", false);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }
    
    [Fact] 
      public async Task GetPostCommentsAsync_ShouldFail_WhenPostNotFound() {
          _postRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Post?)null);
          var result = await _postsService.GetPostCommentsAsync("user-1", "1", false);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetPostCommentsAsync_ShouldReturnUnauthorized_WhenActorMissing()
    {
        var result = await _postsService.GetPostCommentsAsync("", "post-1", false);

        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }
    #endregion
    
    #region CREATE POST
    // --- 2. CREATE POST ---

    [Fact]
    public async Task CreatePostAsync_ShouldReturnUnauthorized_WhenActorUserIdIsMissing()
    {
        // Act
        var result = await _postsService.CreatePostAsync("", new PostCreateRequest { Content = "Test" });

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task CreatePostAsync_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((User?)null);

        // Act
        var result = await _postsService.CreatePostAsync("user-1", new PostCreateRequest { Content = "Test" });

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task CreatePostAsync_ShouldReturnValidation_WhenPrivacyInvalid()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync("u1", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new User { Id = "u1" });

        var result = await _postsService.CreatePostAsync("u1", new PostCreateRequest
        {
            Content = "C",
            Privacy = "Unknown"
        });

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
    }

     [Fact]
      public async Task CreateCommentAsync_ShouldFail_WhenUserContextMissing() {
        var result = await _postsService.CreateCommentAsync("", "post1", new CommentCreateRequest());
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
     public async Task CreateCommentAsync_ShouldFail_WhenPostNotFound() {
        _postRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Post?)null);
        var result = await _postsService.CreateCommentAsync("user1", "post1", new CommentCreateRequest());
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

     [Fact] public async Task CreatePostAsync_ShouldSucceed_WhenDataIsValid() {
        var mockTransaction = new Mock<IDbContextTransaction>();
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                        .ReturnsAsync(mockTransaction.Object);
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new User { Id = "u1" });
        var result = await _postsService.CreatePostAsync("u1", new PostCreateRequest { Content = "C" });
        Assert.True(result.Success);
    }
    #endregion

    #region SHARE POST
    [Fact]
    public async Task SharePostAsync_ShouldReturnUnauthorized_WhenActorMissing()
    {
        var result = await _postsService.SharePostAsync("", "post-1", new PostShareCreateRequest());

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task UnsharePostAsync_ShouldReturnUnauthorized_WhenActorMissing()
    {
        var result = await _postsService.UnsharePostAsync("", "post-1");

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }
    #endregion

    #region UNLIKE POST
    [Fact]
    public async Task UnlikePostAsync_ShouldReturnNotFound_WhenUserMissing()
    {
        _userRepoMock
            .Setup(r => r.ExistsByIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _postsService.UnlikePostAsync("user-1", "post-1");

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }
    #endregion
    
    #region UPDATE POST
    // --- 3. UPDATE POST ---

    [Fact]
    public async Task UpdatePostAsync_ShouldReturnUnauthorized_WhenUserIsNotOwnerOrAdmin()
    {
        // Arrange
        var postId = "post-123";
        var existingPost = new Post { UserId = "owner-id" };
        _postRepoMock.Setup(r => r.GetByIdAsync(postId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(existingPost);

        // Act
        var result = await _postsService.UpdatePostAsync("attacker-id", postId, new PostUpdateRequest(), false);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task UpdatePostAsync_ShouldSucceed_WhenUserIsOwner() {
        var post = new Post { UserId = "u1" };
        _postRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(post);
        var result = await _postsService.UpdatePostAsync("u1", "p1", new PostUpdateRequest { Content = "New" }, false);
        Assert.True(result.Success);
    }
    #endregion

    #region SHARE POST
    [Fact]
    public async Task SharePostAsync_ShouldReturnValidation_WhenPrivacyInvalid()
    {
        var mockTransaction = new Mock<IDbContextTransaction>();
        mockTransaction
            .Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockTransaction.Object);
        _userRepoMock
            .Setup(r => r.GetByIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = "user-1", UserName = "user-1" });
        _postRepoMock
            .Setup(r => r.GetByIdAsync("post-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Post { PostId = "post-1", UserId = "owner", Privacy = PostPrivacy.Public });

        var result = await _postsService.SharePostAsync("user-1", "post-1", new PostShareCreateRequest
        {
            Content = "Share",
            Privacy = "Invalid"
        });

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
        mockTransaction.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    #endregion
    
    #region DELETE POST
    // --- 4. DELETE POST ---

    [Fact]
    public async Task DeletePostAsync_ShouldSucceed_WhenUserIsOwner()
    {
        // Arrange
        var userId = "user-1";
        var postId = "post-1";
        var existingPost = new Post { UserId = userId };
        _postRepoMock.Setup(r => r.GetByIdAsync(postId, It.IsAny<CancellationToken>())).ReturnsAsync(existingPost);

        // Act
        var result = await _postsService.DeletePostAsync(userId, postId, false);

        // Assert
        Assert.True(result.Success);
        _postRepoMock.Verify(r => r.DeleteAsync(postId, It.IsAny<CancellationToken>()), Times.Once);
    }


        [Fact] public async Task DeleteCommentAsync_ShouldReturnOk_WhenDeleted() {
        _commentRepoMock.Setup(r => r.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var result = await _postsService.DeleteCommentAsync("p1", "c1");
        Assert.True(result.Success);
    }

    [Fact]
     public async Task DeleteCommentAsync_ShouldFail_WhenCommentNotFound() {
        _commentRepoMock.Setup(r => r.DeleteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var result = await _postsService.DeleteCommentAsync("p1", "c1");
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }
    #endregion
    
    #region LIKE POST
    // --- 5. LIKE POST (Transaction logic) ---
    [Fact]
    public async Task LikePostAsync_ShouldRollback_WhenPostDoesNotExist()
    {
        // Arrange
        var mockTransaction = new Mock<IDbContextTransaction>();
        _userRepoMock.Setup(r => r.ExistsByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(mockTransaction.Object);
        _postRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Post?)null);

        // Act
        var result = await _postsService.LikePostAsync("user-1", "invalid-post", new LikeCreateRequest());

        // Assert
        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
        mockTransaction.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LikePostAsync_ShouldReturnIsCreatedFalse_WhenAlreadyLiked()
    {
        // Arrange
        var userId = "user-1";
        var postId = "post-1";
        var mockTransaction = new Mock<IDbContextTransaction>();
        var existingLike = new Like { PostId = postId, UserId = userId };
        var post = new Post { PostId = postId, UserId = userId };

        _userRepoMock.Setup(r => r.ExistsByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>())).ReturnsAsync(mockTransaction.Object);
        _postRepoMock.Setup(r => r.GetByIdAsync(postId, It.IsAny<CancellationToken>())).ReturnsAsync(post);
        _likeRepoMock.Setup(r => r.GetByPostAndUserAsync(postId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(existingLike);

        // Act
        var result = await _postsService.LikePostAsync(userId, postId, new LikeCreateRequest());

        // Assert
        Assert.True(result.Success);
        Assert.False(result.Data!.IsCreated);
        mockTransaction.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    #endregion

    #region REPORT POST
    // --- 6. REPORT POST ---
    [Fact]
    public async Task ReportPostAsync_ShouldReturnConflict_WhenAlreadyReported()
    {
        // Arrange
        var userId = "user-1";
        var postId = "post-1";
        _userRepoMock.Setup(r => r.ExistsByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _postRepoMock.Setup(r => r.GetByIdAsync(postId, It.IsAny<CancellationToken>())).ReturnsAsync(new Post());
        _reportRepoMock.Setup(r => r.ExistsByPostAndReporterAsync(postId, userId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _postsService.ReportPostAsync(userId, postId, new PostReportCreateRequest());

        // Assert
        Assert.Equal(ServiceErrorType.Conflict, result.ErrorType);
    }
    #endregion

    // --- ADDITIONAL TESTS TO REACH 15+ ---
    

   



   

  
}