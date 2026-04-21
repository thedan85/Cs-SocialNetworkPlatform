using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public interface IPostShareRepository
{
    Task<PostShare?> GetByPostAndUserAsync(string postId, string userId, CancellationToken ct = default);

    Task<IReadOnlyList<string>> GetSharedPostIdsAsync(
        string userId,
        IReadOnlyCollection<string> postIds,
        CancellationToken ct = default);

    Task<IReadOnlyDictionary<string, int>> GetShareCountsAsync(
        IReadOnlyCollection<string> postIds,
        CancellationToken ct = default);

    Task<int> CountByPostIdAsync(string postId, CancellationToken ct = default);

    Task AddAsync(PostShare share, CancellationToken ct = default);

    Task<bool> DeleteByPostAndUserAsync(string postId, string userId, CancellationToken ct = default);
}
