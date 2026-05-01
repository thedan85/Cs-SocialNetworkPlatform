using Microsoft.AspNetCore.SignalR;
using Moq;
using SocialNetwork.Dtos;
using SocialNetwork.Hubs;
using SocialNetwork.Model;
using SocialNetwork.Repository;
using SocialNetwork.Service;
using Xunit;

namespace SocialNetwork.Tests.Services;

public class NotificationsServiceTests
{
    private readonly Mock<INotificationRepository> _notificationRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IHubContext<NotificationsHub>> _hubContextMock;
    private readonly Mock<IHubClients> _hubClientsMock;
    private readonly Mock<IClientProxy> _clientProxyMock;
    private readonly NotificationsService _notificationsService;

    public NotificationsServiceTests()
    {
        _notificationRepoMock = new Mock<INotificationRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _hubContextMock = new Mock<IHubContext<NotificationsHub>>();
        _hubClientsMock = new Mock<IHubClients>();
        _clientProxyMock = new Mock<IClientProxy>();

        _hubClientsMock
            .Setup(clients => clients.Group(It.IsAny<string>()))
            .Returns(_clientProxyMock.Object);
        _hubContextMock
            .SetupGet(context => context.Clients)
            .Returns(_hubClientsMock.Object);
        _clientProxyMock
            .Setup(proxy => proxy.SendCoreAsync(
                "notification:created",
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _notificationsService = new NotificationsService(
            _notificationRepoMock.Object,
            _userRepoMock.Object,
            _hubContextMock.Object);
    }

    [Fact]
    public async Task GetNotificationsAsync_ShouldReturnNotFound_WhenUserMissing()
    {
        _userRepoMock
            .Setup(repo => repo.ExistsByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _notificationsService.GetNotificationsAsync("missing-user");

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetUnreadNotificationsAsync_ShouldReturnOk_WhenUserExists()
    {
        _userRepoMock
            .Setup(repo => repo.ExistsByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _notificationRepoMock
            .Setup(repo => repo.GetByRecipientUserIdAsync(
                It.IsAny<string>(),
                false,
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Notification>
            {
                new()
                {
                    NotificationId = "n1",
                    RecipientUserId = "user-1",
                    SenderUserId = "sender-1",
                    Content = "Unread",
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                }
            });

        var result = await _notificationsService.GetUnreadNotificationsAsync("user-1");

        Assert.True(result.Success);
        Assert.Single(result.Data!);
    }

    [Fact]
    public async Task CreateNotificationAsync_ShouldReturnValidation_WhenRecipientIsSender()
    {
        var result = await _notificationsService.CreateNotificationAsync(
            "user-1",
            new NotificationCreateRequest { RecipientUserId = "user-1" });

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public async Task CreateNotificationAsync_ShouldReturnNotFound_WhenUsersMissing()
    {
        _userRepoMock
            .Setup(repo => repo.ExistsByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _notificationsService.CreateNotificationAsync(
            "sender-1",
            new NotificationCreateRequest { RecipientUserId = "recipient-1" });

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task CreateNotificationAsync_ShouldSendHubEvent_WhenValid()
    {
        _userRepoMock
            .Setup(repo => repo.ExistsByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _notificationsService.CreateNotificationAsync(
            "sender-1",
            new NotificationCreateRequest
            {
                RecipientUserId = "recipient-1",
                Type = "Like",
                Content = "Liked your post."
            });

        Assert.True(result.Success);
        _notificationRepoMock.Verify(
            repo => repo.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _clientProxyMock.Verify(
            proxy => proxy.SendCoreAsync(
                "notification:created",
                It.IsAny<object[]>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MarkAsReadAsync_ShouldReturnUnauthorized_WhenActorMissing()
    {
        var result = await _notificationsService.MarkAsReadAsync("", "notification-1", false);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task MarkAsReadAsync_ShouldReturnUnauthorized_WhenNotOwnerOrAdmin()
    {
        _notificationRepoMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Notification
            {
                NotificationId = "notification-1",
                RecipientUserId = "owner-1",
                SenderUserId = "sender-1",
                CreatedAt = DateTime.UtcNow
            });

        var result = await _notificationsService.MarkAsReadAsync("actor-1", "notification-1", false);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task MarkAsReadAsync_ShouldReturnOk_WhenUpdated()
    {
        var notification = new Notification
        {
            NotificationId = "notification-1",
            RecipientUserId = "owner-1",
            SenderUserId = "sender-1",
            CreatedAt = DateTime.UtcNow
        };

        _notificationRepoMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(notification);
        _notificationRepoMock
            .Setup(repo => repo.MarkAsReadAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Notification
            {
                NotificationId = notification.NotificationId,
                RecipientUserId = notification.RecipientUserId,
                SenderUserId = notification.SenderUserId,
                CreatedAt = notification.CreatedAt,
                IsRead = true
            });

        var result = await _notificationsService.MarkAsReadAsync("owner-1", "notification-1", false);

        Assert.True(result.Success);
        Assert.True(result.Data!.IsRead);
    }

    [Fact]
    public async Task DeleteNotificationAsync_ShouldReturnUnauthorized_WhenNotOwnerOrAdmin()
    {
        _notificationRepoMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Notification
            {
                NotificationId = "notification-1",
                RecipientUserId = "owner-1",
                SenderUserId = "sender-1",
                CreatedAt = DateTime.UtcNow
            });

        var result = await _notificationsService.DeleteNotificationAsync("actor-1", "notification-1", false);

        Assert.False(result.Success);
        Assert.Equal(ServiceErrorType.Unauthorized, result.ErrorType);
    }

    [Fact]
    public async Task DeleteNotificationAsync_ShouldReturnOk_WhenDeleted()
    {
        _notificationRepoMock
            .Setup(repo => repo.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Notification
            {
                NotificationId = "notification-1",
                RecipientUserId = "owner-1",
                SenderUserId = "sender-1",
                CreatedAt = DateTime.UtcNow
            });
        _notificationRepoMock
            .Setup(repo => repo.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _notificationsService.DeleteNotificationAsync("owner-1", "notification-1", false);

        Assert.True(result.Success);
        Assert.Equal("Notification deleted.", result.Data);
    }
}
