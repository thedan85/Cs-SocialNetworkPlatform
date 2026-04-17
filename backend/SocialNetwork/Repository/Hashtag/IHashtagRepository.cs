using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public interface IHashtagRepository
{
    Task<IReadOnlyList<Hashtag>> AddOrUpdateAsync(IReadOnlyList<string> tags, CancellationToken ct = default);

    Task<IReadOnlyList<Hashtag>> SearchAsync(
        string query,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task AddPostHashtagsAsync(string postId, IEnumerable<string> hashtagIds, CancellationToken ct = default);
}
