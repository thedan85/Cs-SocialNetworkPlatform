using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public interface IStoryRepository
{
    Task<IReadOnlyList<Story>> GetActiveAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<Story?> GetByIdAsync(string storyId, CancellationToken ct = default);

    Task<IReadOnlyList<Story>> GetActiveByUserIdAsync(
        string userId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task AddAsync(Story story, CancellationToken ct = default);
    Task<bool> DeleteAsync(string storyId, CancellationToken ct = default);
}
