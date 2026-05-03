using SocialNetwork.Dtos;

namespace SocialNetwork.Service;

public interface IFriendsService
{
    Task<ServiceResult<FriendshipResponse>> CreateFriendRequestAsync(
        string requesterUserId,
        FriendRequestCreateRequest request,
        CancellationToken ct = default);
    Task<ServiceResult<FriendshipResponse>> AcceptFriendRequestAsync(
        string actorUserId,
        string friendshipId,
        bool isAdmin,
        CancellationToken ct = default);
    Task<ServiceResult<FriendshipResponse>> RejectFriendRequestAsync(
        string actorUserId,
        string friendshipId,
        bool isAdmin,
        CancellationToken ct = default);

    Task<ServiceResult<FriendRelationshipResponse>> GetRelationshipAsync(
        string actorUserId,
        string targetUserId,
        CancellationToken ct = default);

    Task<ServiceResult<string>> RemoveFriendAsync(
        string actorUserId,
        string friendshipId,
        bool isAdmin,
        CancellationToken ct = default);

    Task<ServiceResult<IReadOnlyList<FriendshipResponse>>> GetFriendsAsync(
        string userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default);

    Task<ServiceResult<IReadOnlyList<FriendshipResponse>>> GetPendingRequestsAsync(
        string userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default);
}
