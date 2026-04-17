using SocialNetwork.Dtos;

namespace SocialNetwork.Service;

public interface IHashtagsService
{
    Task<ServiceResult<IReadOnlyList<HashtagSearchResponse>>> SearchHashtagsAsync(
        string query,
        int pageNumber = 1,
        int pageSize = 10,
        int postsPerHashtag = 3,
        CancellationToken ct = default);
}
