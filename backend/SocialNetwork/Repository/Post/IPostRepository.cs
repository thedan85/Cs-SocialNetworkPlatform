using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(string postId, CancellationToken ct = default);

    Task<IReadOnlyList<Post>> GetByUserIdAsync(
        string userId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<int> CountByUserIdAsync(string userId, CancellationToken ct = default);

    Task AddAsync(Post post, CancellationToken ct = default);

    Task UpdateAsync(Post post, CancellationToken ct = default);

    Task DeleteAsync(string postId, CancellationToken ct = default);
}
