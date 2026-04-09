using SocialNetwork.Dtos;

namespace SocialNetwork.Service;

public interface IFriendsService
{
    Task<ServiceResult<FriendshipResponse>> CreateFriendRequestAsync(
        string requesterUserId,
        FriendRequestCreateRequest request,
        CancellationToken ct = default);
    Task<ServiceResult<FriendshipResponse>> AcceptFriendRequestAsync(string friendshipId, CancellationToken ct = default);
    Task<ServiceResult<FriendshipResponse>> RejectFriendRequestAsync(string friendshipId, CancellationToken ct = default);

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
