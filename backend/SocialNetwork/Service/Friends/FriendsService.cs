using SocialNetwork.Dtos;
using SocialNetwork.Extensions;
using SocialNetwork.Helpers;
using SocialNetwork.Model;
using SocialNetwork.Repository;

namespace SocialNetwork.Service;

public class FriendsService : IFriendsService
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;

    public FriendsService(
        IFriendshipRepository friendshipRepository,
        IUserRepository userRepository,
        INotificationRepository notificationRepository)
    {
        _friendshipRepository = friendshipRepository;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
    }

    public async Task<ServiceResult<FriendshipResponse>> CreateFriendRequestAsync(
        FriendRequestCreateRequest request,
        CancellationToken ct = default)
    {
        if (request.RequesterUserId == request.AddresseeUserId)
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.Validation, "You cannot friend yourself.");
        }

        var requesterExists = await _userRepository.ExistsByIdAsync(request.RequesterUserId, ct);
        var addresseeExists = await _userRepository.ExistsByIdAsync(request.AddresseeUserId, ct);

        if (!requesterExists || !addresseeExists)
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.NotFound, "One or more users were not found.");
        }

        var requestExists = await _friendshipRepository.ExistsBetweenUsersAsync(
            request.RequesterUserId,
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
            UserId1 = request.RequesterUserId,
            UserId2 = request.AddresseeUserId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _friendshipRepository.AddAsync(friendship, ct);

        var requester = await _userRepository.GetByIdAsync(request.RequesterUserId, ct);
        var requesterDisplayName = requester?.UserName ?? "A user";

        var notification = new Notification
        {
            RecipientUserId = request.AddresseeUserId,
            SenderUserId = request.RequesterUserId,
            Type = "FriendRequest",
            Content = NotificationContentHelper.BuildFriendRequestContent(requesterDisplayName),
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await _notificationRepository.AddAsync(notification, ct);

        return ServiceResult<FriendshipResponse>.Ok(friendship.ToFriendshipResponse());
    }

    public async Task<ServiceResult<FriendshipResponse>> AcceptFriendRequestAsync(
        string friendshipId,
        CancellationToken ct = default)
    {
        var friendship = await _friendshipRepository.GetByIdAsync(friendshipId, ct);
        if (friendship is null)
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.NotFound, "Friend request not found.");
        }

        if (!string.Equals(friendship.Status, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<FriendshipResponse>.Fail(
                ServiceErrorType.Validation,
                "Only pending requests can be accepted.");
        }

        var updated = await _friendshipRepository.UpdateStatusAsync(friendshipId, "Accepted", ct);
        if (!updated)
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.NotFound, "Friend request not found.");
        }

        friendship.Status = "Accepted";
        friendship.UpdatedAt = DateTime.UtcNow;

        var accepter = await _userRepository.GetByIdAsync(friendship.UserId2, ct);
        var accepterDisplayName = accepter?.UserName ?? "A user";

        var notification = new Notification
        {
            RecipientUserId = friendship.UserId1,
            SenderUserId = friendship.UserId2,
            Type = "FriendAccepted",
            Content = NotificationContentHelper.BuildFriendAcceptedContent(accepterDisplayName),
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await _notificationRepository.AddAsync(notification, ct);

        return ServiceResult<FriendshipResponse>.Ok(friendship.ToFriendshipResponse());
    }

    public async Task<ServiceResult<FriendshipResponse>> RejectFriendRequestAsync(
        string friendshipId,
        CancellationToken ct = default)
    {
        var friendship = await _friendshipRepository.GetByIdAsync(friendshipId, ct);
        if (friendship is null)
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.NotFound, "Friend request not found.");
        }

        if (!string.Equals(friendship.Status, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<FriendshipResponse>.Fail(
                ServiceErrorType.Validation,
                "Only pending requests can be rejected.");
        }

        var updated = await _friendshipRepository.UpdateStatusAsync(friendshipId, "Rejected", ct);
        if (!updated)
        {
            return ServiceResult<FriendshipResponse>.Fail(ServiceErrorType.NotFound, "Friend request not found.");
        }

        friendship.Status = "Rejected";
        friendship.UpdatedAt = DateTime.UtcNow;

        return ServiceResult<FriendshipResponse>.Ok(friendship.ToFriendshipResponse());
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
