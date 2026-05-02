using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using SocialNetwork.Dtos;
using SocialNetwork.Hubs;
using SocialNetwork.Model;
using SocialNetwork.Repository;
using SocialNetwork.Service;
using Xunit;

namespace SocialNetwork.Tests.Services;

public class FriendsServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IFriendshipRepository> _friendshipRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<INotificationRepository> _notificationRepoMock;
    private readonly Mock<IHubContext<NotificationsHub>> _hubContextMock;
    private readonly Mock<IHubClients> _hubClientsMock;
    private readonly Mock<IClientProxy> _clientProxyMock;
    private readonly Mock<IDbContextTransaction> _transactionMock;
    private readonly FriendsService _friendsService;

    public FriendsServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _friendshipRepoMock = new Mock<IFriendshipRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _notificationRepoMock = new Mock<INotificationRepository>();
        _hubContextMock = new Mock<IHubContext<NotificationsHub>>();
        _hubClientsMock = new Mock<IHubClients>();
        _clientProxyMock = new Mock<IClientProxy>();
        _transactionMock = new Mock<IDbContextTransaction>();

        _transactionMock
            .Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _transactionMock
            .Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_transactionMock.Object);

        _hubClientsMock
            .Setup(c => c.Group(It.IsAny<string>()))
            .Returns(_clientProxyMock.Object);
        _hubContextMock
            .SetupGet(c => c.Clients)
            .Returns(_hubClientsMock.Object);
        _clientProxyMock
            .Setup(p => p.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _friendshipRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Friendship>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _notificationRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _friendsService = new FriendsService(
            _unitOfWorkMock.Object,
            _friendshipRepoMock.Object,
            _userRepoMock.Object,
            _notificationRepoMock.Object,
            _hubContextMock.Object);
    }

    [Fact]
    public async Task CreateFriendRequestAsync_ShouldReturnUnauthorized_WhenRequesterMissing()
    {
        var result = await _friendsService.CreateFriendRequestAsync(
            "",
            new FriendRequestCreateRequest { AddresseeUserId = "user-2" });

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task CreateFriendRequestAsync_ShouldReturnValidation_WhenRequestingSelf()
    {
        var result = await _friendsService.CreateFriendRequestAsync(
            "user-1",
            new FriendRequestCreateRequest { AddresseeUserId = "user-1" });

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public async Task CreateFriendRequestAsync_ShouldReturnNotFound_WhenUserMissing()
    {
        _userRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _friendsService.CreateFriendRequestAsync(
            "user-1",
            new FriendRequestCreateRequest { AddresseeUserId = "user-2" });

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task CreateFriendRequestAsync_ShouldReturnConflict_WhenRequestExists()
    {
        var requesterId = "user-1";
        var addresseeId = "user-2";
        var requester = new User { Id = requesterId, UserName = "requester" };
        var addressee = new User { Id = addresseeId, UserName = "addressee" };

        _userRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string id, CancellationToken _) => id == requesterId ? requester : addressee);
        _friendshipRepoMock
            .Setup(r => r.ExistsBetweenUsersAsync(requesterId, addresseeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _friendsService.CreateFriendRequestAsync(
            requesterId,
            new FriendRequestCreateRequest { AddresseeUserId = addresseeId });

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Conflict, result.ErrorType);
    }

    [Fact]
    public async Task CreateFriendRequestAsync_ShouldCreateFriendRequest_WhenValid()
    {
        var requesterId = "user-1";
        var addresseeId = "user-2";
        var requester = new User { Id = requesterId, UserName = "requester" };
        var addressee = new User { Id = addresseeId, UserName = "addressee" };

        _userRepoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string id, CancellationToken _) => id == requesterId ? requester : addressee);
        _friendshipRepoMock
            .Setup(r => r.ExistsBetweenUsersAsync(requesterId, addresseeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _friendsService.CreateFriendRequestAsync(
            requesterId,
            new FriendRequestCreateRequest { AddresseeUserId = addresseeId });

        Assert.True(result.Success);
        Assert.Equal(requesterId, result.Data!.UserId1);
        Assert.Equal(addresseeId, result.Data.UserId2);
        _friendshipRepoMock.Verify(
            r => r.AddAsync(It.IsAny<Friendship>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _notificationRepoMock.Verify(
            r => r.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AcceptFriendRequestAsync_ShouldReturnUnauthorized_WhenActorMissing()
    {
        var result = await _friendsService.AcceptFriendRequestAsync("", "friendship-1", false);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task AcceptFriendRequestAsync_ShouldReturnNotFound_WhenRequestMissing()
    {
        _friendshipRepoMock
            .Setup(r => r.GetByIdAsync("friendship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Friendship?)null);

        var result = await _friendsService.AcceptFriendRequestAsync("user-2", "friendship-1", false);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task AcceptFriendRequestAsync_ShouldReturnUnauthorized_WhenNotAddresseeAndNotAdmin()
    {
        var friendship = new Friendship
        {
            FriendshipId = "friendship-1",
            UserId1 = "user-1",
            UserId2 = "user-2",
            Status = "Pending"
        };

        _friendshipRepoMock
            .Setup(r => r.GetByIdAsync("friendship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(friendship);

        var result = await _friendsService.AcceptFriendRequestAsync("user-3", "friendship-1", false);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task AcceptFriendRequestAsync_ShouldReturnValidation_WhenNotPending()
    {
        var friendship = new Friendship
        {
            FriendshipId = "friendship-1",
            UserId1 = "user-1",
            UserId2 = "user-2",
            Status = "Accepted"
        };

        _friendshipRepoMock
            .Setup(r => r.GetByIdAsync("friendship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(friendship);

        var result = await _friendsService.AcceptFriendRequestAsync("user-2", "friendship-1", false);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public async Task AcceptFriendRequestAsync_ShouldReturnNotFound_WhenUpdateFails()
    {
        var friendship = new Friendship
        {
            FriendshipId = "friendship-1",
            UserId1 = "user-1",
            UserId2 = "user-2",
            Status = "Pending"
        };

        _friendshipRepoMock
            .Setup(r => r.GetByIdAsync("friendship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(friendship);
        _friendshipRepoMock
            .Setup(r => r.UpdateStatusAsync(
                "friendship-1",
                "Accepted",
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _friendsService.AcceptFriendRequestAsync("user-2", "friendship-1", false);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
        _transactionMock.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AcceptFriendRequestAsync_ShouldReturnOk_WhenValid()
    {
        var friendship = new Friendship
        {
            FriendshipId = "friendship-1",
            UserId1 = "user-1",
            UserId2 = "user-2",
            Status = "Pending"
        };

        _friendshipRepoMock
            .Setup(r => r.GetByIdAsync("friendship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(friendship);
        _friendshipRepoMock
            .Setup(r => r.UpdateStatusAsync(
                "friendship-1",
                "Accepted",
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _userRepoMock
            .Setup(r => r.GetByIdAsync("user-2", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = "user-2", UserName = "acceptor" });

        var result = await _friendsService.AcceptFriendRequestAsync("user-2", "friendship-1", false);

        Assert.True(result.Success);
        Assert.Equal("Accepted", result.Data!.Status);
        _notificationRepoMock.Verify(
            r => r.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _transactionMock.Verify(t => t.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RejectFriendRequestAsync_ShouldReturnUnauthorized_WhenActorMissing()
    {
        var result = await _friendsService.RejectFriendRequestAsync("", "friendship-1", false);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task RejectFriendRequestAsync_ShouldReturnValidation_WhenNotPending()
    {
        var friendship = new Friendship
        {
            FriendshipId = "friendship-1",
            UserId1 = "user-1",
            UserId2 = "user-2",
            Status = "Rejected"
        };

        _friendshipRepoMock
            .Setup(r => r.GetByIdAsync("friendship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(friendship);

        var result = await _friendsService.RejectFriendRequestAsync("user-2", "friendship-1", false);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public async Task RejectFriendRequestAsync_ShouldReturnOk_WhenValid()
    {
        var friendship = new Friendship
        {
            FriendshipId = "friendship-1",
            UserId1 = "user-1",
            UserId2 = "user-2",
            Status = "Pending"
        };

        _friendshipRepoMock
            .Setup(r => r.GetByIdAsync("friendship-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(friendship);
        _friendshipRepoMock
            .Setup(r => r.UpdateStatusAsync(
                "friendship-1",
                "Rejected",
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _friendsService.RejectFriendRequestAsync("user-2", "friendship-1", false);

        Assert.True(result.Success);
        Assert.Equal("Rejected", result.Data!.Status);
    }

    [Fact]
    public async Task GetFriendsAsync_ShouldReturnNotFound_WhenUserMissing()
    {
        _userRepoMock
            .Setup(r => r.ExistsByIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _friendsService.GetFriendsAsync("user-1");

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetFriendsAsync_ShouldReturnOk_WhenUserExists()
    {
        _userRepoMock
            .Setup(r => r.ExistsByIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _friendshipRepoMock
            .Setup(r => r.GetAcceptedFriendshipsForUserAsync(
                "user-1",
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Friendship> { new Friendship { UserId1 = "user-1", UserId2 = "user-2" } });

        var result = await _friendsService.GetFriendsAsync("user-1");

        Assert.True(result.Success);
        Assert.Single(result.Data!);
    }

    [Fact]
    public async Task GetPendingRequestsAsync_ShouldReturnNotFound_WhenUserMissing()
    {
        _userRepoMock
            .Setup(r => r.ExistsByIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _friendsService.GetPendingRequestsAsync("user-1");

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetPendingRequestsAsync_ShouldReturnOk_WhenUserExists()
    {
        _userRepoMock
            .Setup(r => r.ExistsByIdAsync("user-1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _friendshipRepoMock
            .Setup(r => r.GetPendingRequestsForUserAsync(
                "user-1",
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Friendship> { new Friendship { UserId1 = "user-3", UserId2 = "user-1" } });

        var result = await _friendsService.GetPendingRequestsAsync("user-1");

        Assert.True(result.Success);
        Assert.Single(result.Data!);
    }
}
