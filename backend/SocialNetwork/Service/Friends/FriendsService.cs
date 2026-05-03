using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Dtos;
using SocialNetwork.Extensions;
using SocialNetwork.Helpers;
using SocialNetwork.Hubs;
using SocialNetwork.Model;
using SocialNetwork.Repository;

namespace SocialNetwork.Service;

public class FriendsService : IFriendsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationsHub> _notificationsHub;

    public FriendsService(
        IUnitOfWork unitOfWork,
        IFriendshipRepository friendshipRepository,
        IUserRepository userRepository,
        INotificationRepository notificationRepository,
        IHubContext<NotificationsHub> notificationsHub)
    {
        _unitOfWork = unitOfWork;
        _friendshipRepository = friendshipRepository;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
        _notificationsHub = notificationsHub;
    }

    public async Task<ServiceResult<FriendshipResponse>> CreateFriendRequestAsync(
        string requesterUserId,
        FriendRequestCreateRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(requesterUserId))
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.Unauthorized, "User context is missing.");
        }

        if (requesterUserId == request.AddresseeUserId)
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.Validation, "You cannot friend yourself.");
        }

        var requester = await _userRepository.GetByIdAsync(requesterUserId, ct);
        var addressee = await _userRepository.GetByIdAsync(request.AddresseeUserId, ct);

        if (requester is null || addressee is null)
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.NotFound, "One or more users were not found.");
        }

        var requestExists = await _friendshipRepository.ExistsBetweenUsersAsync(
            requesterUserId,
            request.AddresseeUserId,
            ct);

        if (requestExists)
        {
            return ServiceResult<FriendshipResponse>.Fail(
                ServiceErrorType.Conflict,
                "A friend request already exists between these users.");
        }

        var friendship = new Friendship
        {
            UserId1 = requesterUserId,
            UserId2 = request.AddresseeUserId,
            Status = "Pending"
        };

        var now = DateTime.UtcNow;
        friendship.CreatedAt = now;

        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            await _friendshipRepository.AddAsync(friendship, ct);

            var requesterDisplayName = requester.UserName ?? $"User {requesterUserId}";

            var notification = new Notification
            {
                RecipientUserId = request.AddresseeUserId,
                SenderUserId = requesterUserId,
                Type = "FriendRequest",
                Content = NotificationContentHelper.BuildFriendRequestContent(requesterDisplayName),
                CreatedAt = now,
                IsRead = false
            };

            await _notificationRepository.AddAsync(notification, ct);
            var notificationResponse = notification.ToNotificationResponse();
            await _notificationsHub.Clients
                .Group(NotificationsHub.GroupName(notification.RecipientUserId))
                .SendAsync("notification:created", notificationResponse, ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }

        var response = new FriendshipResponse
        {
            FriendshipId = friendship.FriendshipId,
            UserId1 = friendship.UserId1,
            UserId2 = friendship.UserId2,
            User1Name = requester.UserName,
            User1FirstName = requester.FirstName,
            User1LastName = requester.LastName,
            User2Name = addressee.UserName,
            User2FirstName = addressee.FirstName,
            User2LastName = addressee.LastName,
            Status = friendship.Status,
            CreatedAt = friendship.CreatedAt,
            UpdatedAt = friendship.UpdatedAt
        };

        return ServiceResult<FriendshipResponse>.Ok(response);
    }

    public async Task<ServiceResult<FriendshipResponse>> AcceptFriendRequestAsync(
        string actorUserId,
        string friendshipId,
        bool isAdmin,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.Unauthorized, "User context is missing.");
        }

        var friendship = await _friendshipRepository.GetByIdAsync(friendshipId, ct);
        if (friendship is null)
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.NotFound, "Friend request not found.");
        }

        if (!isAdmin && !string.Equals(friendship.UserId2, actorUserId, StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.Unauthorized, "You are not allowed to accept this request.");
        }

        if (!string.Equals(friendship.Status, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<FriendshipResponse>.Fail(
                ServiceErrorType.Validation,
                "Only pending requests can be accepted.");
        }

        var now = DateTime.UtcNow;

        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var updated = await _friendshipRepository.UpdateStatusAsync(friendshipId, "Accepted", now, ct);
            if (!updated)
            {
                await transaction.RollbackAsync(ct);
                return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.NotFound, "Friend request not found.");
            }

            var accepter = await _userRepository.GetByIdAsync(actorUserId, ct);
            var accepterDisplayName = accepter?.UserName ?? $"User {actorUserId}";

            var notification = new Notification
            {
                RecipientUserId = friendship.UserId1,
                SenderUserId = actorUserId,
                Type = "FriendAccepted",
                Content = NotificationContentHelper.BuildFriendAcceptedContent(accepterDisplayName),
                CreatedAt = now,
                IsRead = false
            };

            await _notificationRepository.AddAsync(notification, ct);
            var notificationResponse = notification.ToNotificationResponse();
            await _notificationsHub.Clients
                .Group(NotificationsHub.GroupName(notification.RecipientUserId))
                .SendAsync("notification:created", notificationResponse, ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }

        friendship.Status = "Accepted";
        friendship.UpdatedAt = now;

        return ServiceResult<FriendshipResponse>.Ok(friendship.ToFriendshipResponse());
    }

    public async Task<ServiceResult<FriendshipResponse>> RejectFriendRequestAsync(
        string actorUserId,
        string friendshipId,
        bool isAdmin,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.Unauthorized, "User context is missing.");
        }

        var friendship = await _friendshipRepository.GetByIdAsync(friendshipId, ct);
        if (friendship is null)
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.NotFound, "Friend request not found.");
        }

        if (!isAdmin && !string.Equals(friendship.UserId2, actorUserId, StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.Unauthorized, "You are not allowed to reject this request.");
        }

        if (!string.Equals(friendship.Status, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<FriendshipResponse>.Fail(
                ServiceErrorType.Validation,
                "Only pending requests can be rejected.");
        }

        var now = DateTime.UtcNow;

        var updated = await _friendshipRepository.UpdateStatusAsync(friendshipId, "Rejected", now, ct);
        if (!updated)
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.NotFound, "Friend request not found.");
        }

        friendship.Status = "Rejected";
        friendship.UpdatedAt = now;

        return ServiceResult<FriendshipResponse>.Ok(friendship.ToFriendshipResponse());
    }

    public async Task<ServiceResult<FriendRelationshipResponse>> GetRelationshipAsync(
        string actorUserId,
        string targetUserId,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<FriendRelationshipResponse>.Fail(
                ServiceErrorType.Unauthorized,
                "User context is missing.");
        }

        if (string.IsNullOrWhiteSpace(targetUserId))
        {
            return ServiceResult<FriendRelationshipResponse>.Fail(
                ServiceErrorType.Validation,
                "Target user is required.");
        }

        var targetExists = await _userRepository.ExistsByIdAsync(targetUserId, ct);
        if (!targetExists)
        {
            return ServiceResult<FriendRelationshipResponse>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var friendship = await _friendshipRepository.GetBetweenUsersAsync(actorUserId, targetUserId, ct);
        if (friendship is null)
        {
            return ServiceResult<FriendRelationshipResponse>.Ok(new FriendRelationshipResponse());
        }

        var isRequester = string.Equals(friendship.UserId1, actorUserId, StringComparison.OrdinalIgnoreCase);
        var isAddressee = string.Equals(friendship.UserId2, actorUserId, StringComparison.OrdinalIgnoreCase);

        var response = new FriendRelationshipResponse
        {
            Status = friendship.Status,
            FriendshipId = friendship.FriendshipId,
            RequesterUserId = friendship.UserId1,
            AddresseeUserId = friendship.UserId2,
            IsRequester = isRequester,
            IsAddressee = isAddressee
        };

        return ServiceResult<FriendRelationshipResponse>.Ok(response);
    }

    public async Task<ServiceResult<string>> RemoveFriendAsync(
        string actorUserId,
        string friendshipId,
        bool isAdmin,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<string>.Fail(ServiceErrorType.Unauthorized, "User context is missing.");
        }

        if (string.IsNullOrWhiteSpace(friendshipId))
        {
            return ServiceResult<string>.Fail(ServiceErrorType.Validation, "Friendship id is required.");
        }

        var friendship = await _friendshipRepository.GetByIdAsync(friendshipId, ct);
        if (friendship is null)
        {
            return ServiceResult<string>.Fail(ServiceErrorType.NotFound, "Friendship not found.");
        }

        if (!isAdmin &&
            !string.Equals(friendship.UserId1, actorUserId, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(friendship.UserId2, actorUserId, StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<string>.Fail(ServiceErrorType.Unauthorized, "You are not allowed to remove this friendship.");
        }

        if (!string.Equals(friendship.Status, "Accepted", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<string>.Fail(
                ServiceErrorType.Validation,
                "Only accepted friendships can be removed.");
        }

        await _friendshipRepository.DeleteAsync(friendshipId, ct);
        return ServiceResult<string>.Ok("Friend removed.");
    }

    public async Task<ServiceResult<IReadOnlyList<FriendshipResponse>>> GetFriendsAsync(
        string userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        var userExists = await _userRepository.ExistsByIdAsync(userId, ct);
        if (!userExists)
        {
            return ServiceResult<IReadOnlyList<FriendshipResponse>>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var friendships = await _friendshipRepository.GetAcceptedFriendshipsForUserAsync(
            userId,
            pageNumber: pageNumber,
            pageSize: pageSize,
            ct: ct);

        var responses = friendships
            .Select(friendship => friendship.ToFriendshipResponse())
            .ToList();

        return ServiceResult<IReadOnlyList<FriendshipResponse>>.Ok(responses);
    }

    public async Task<ServiceResult<IReadOnlyList<FriendshipResponse>>> GetPendingRequestsAsync(
        string userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        var userExists = await _userRepository.ExistsByIdAsync(userId, ct);
        if (!userExists)
        {
            return ServiceResult<IReadOnlyList<FriendshipResponse>>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var friendships = await _friendshipRepository.GetPendingRequestsForUserAsync(
            userId,
            pageNumber: pageNumber,
            pageSize: pageSize,
            ct: ct);

        var responses = friendships
            .Select(friendship => friendship.ToFriendshipResponse())
            .ToList();

        return ServiceResult<IReadOnlyList<FriendshipResponse>>.Ok(responses);
    }
}
