using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public interface ILikeRepository
{
    Task<Like?> GetByPostAndUserAsync(string postId, string userId, CancellationToken ct = default);
    Task AddAsync(Like like, CancellationToken ct = default);
}
