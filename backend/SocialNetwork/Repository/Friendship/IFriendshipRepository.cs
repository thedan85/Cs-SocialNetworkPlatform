using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public interface IFriendshipRepository
{
    Task<Friendship?> GetByIdAsync(string friendshipId, CancellationToken ct = default);

    Task<Friendship?> GetBetweenUsersAsync(string userId1, string userId2, CancellationToken ct = default);

    Task<bool> ExistsBetweenUsersAsync(string userId1, string userId2, CancellationToken ct = default);

    Task<IReadOnlyList<Friendship>> GetPendingRequestsForUserAsync(
        string userId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<IReadOnlyList<Friendship>> GetAcceptedFriendshipsForUserAsync(
        string userId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task AddAsync(Friendship friendship, CancellationToken ct = default);

    Task<bool> UpdateStatusAsync(
        string friendshipId,
        string status,
        DateTime updatedAt,
        CancellationToken ct = default);

    Task DeleteAsync(string friendshipId, CancellationToken ct = default);
}
