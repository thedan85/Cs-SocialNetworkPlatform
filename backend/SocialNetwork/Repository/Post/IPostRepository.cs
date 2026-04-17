using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public interface IPostRepository
{
    Task<IReadOnlyList<Post>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<IReadOnlyList<Post>> GetVisiblePagedAsync(
        string viewerUserId,
        bool isAdmin,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<Post?> GetByIdAsync(string postId, CancellationToken ct = default);

    Task<bool> ExistsByIdAsync(string postId, CancellationToken ct = default);

    Task<IReadOnlyList<Post>> GetByUserIdOrderedAsync(string userId, CancellationToken ct = default);

    Task<IReadOnlyList<Post>> GetByUserIdWithPrivacyAsync(
        string userId,
        IReadOnlyList<string> privacyValues,
        CancellationToken ct = default);

    Task<IReadOnlyList<Post>> GetByUserIdAsync(
        string userId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<int> CountByUserIdAsync(string userId, CancellationToken ct = default);

    Task<bool> IncrementLikeCountAsync(string postId, int delta, CancellationToken ct = default);

    Task AddAsync(Post post, CancellationToken ct = default);

    Task UpdateAsync(Post post, CancellationToken ct = default);

    Task DeleteAsync(string postId, CancellationToken ct = default);
}
