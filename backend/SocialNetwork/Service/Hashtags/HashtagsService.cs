using SocialNetwork.Dtos;
using SocialNetwork.Extensions;
using SocialNetwork.Repository;

namespace SocialNetwork.Service;

public class HashtagsService : IHashtagsService
{
    private readonly IHashtagRepository _hashtagRepository;

    public HashtagsService(IHashtagRepository hashtagRepository)
    {
        _hashtagRepository = hashtagRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<HashtagSearchResponse>>> SearchHashtagsAsync(
        string query,
        int pageNumber = 1,
        int pageSize = 10,
        int postsPerHashtag = 3,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return ServiceResult<IReadOnlyList<HashtagSearchResponse>>.Ok(Array.Empty<HashtagSearchResponse>());
        }

        var hashtags = await _hashtagRepository.SearchAsync(query, pageNumber, pageSize, ct);

        var responses = hashtags
            .Select(hashtag => new HashtagSearchResponse
            {
                HashtagId = hashtag.HashtagId,
                Tag = hashtag.Tag,
                UsageCount = hashtag.UsageCount,
                Posts = hashtag.PostHashtags
                    .Select(postHashtag => postHashtag.Post)
                    .Where(post => post != null)
                    .OrderByDescending(post => post!.CreatedAt)
                    .Take(postsPerHashtag)
                    .Select(post => post!.ToPostResponse())
                    .ToList()
            })
            .ToList();

        return ServiceResult<IReadOnlyList<HashtagSearchResponse>>.Ok(responses);
    }
}
