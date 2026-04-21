using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public interface ILikeRepository
{
    Task<Like?> GetByPostAndUserAsync(string postId, string userId, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetLikedPostIdsAsync(
        string userId,
        IReadOnlyCollection<string> postIds,
        CancellationToken ct = default);
    Task AddAsync(Like like, CancellationToken ct = default);
    Task<bool> DeleteByPostAndUserAsync(string postId, string userId, CancellationToken ct = default);
}
