using Microsoft.EntityFrameworkCore;
using SocialNetwork.Extensions;
using SocialNetwork.Data;
using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public class HashtagRepository : IHashtagRepository
{
    private readonly ApplicationDbContext _dbContext;

    public HashtagRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Hashtag>> AddOrUpdateAsync(
        IReadOnlyList<string> tags,
        CancellationToken ct = default)
    {
        if (tags.Count == 0)
        {
            return Array.Empty<Hashtag>();
        }

        var normalizedTags = tags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();

        if (normalizedTags.Count == 0)
        {
            return Array.Empty<Hashtag>();
        }

        var existing = await _dbContext.Hashtags
            .Where(hashtag => normalizedTags.Contains(hashtag.Tag))
            .ToListAsync(ct);

        var existingTags = new HashSet<string>(existing.Select(hashtag => hashtag.Tag));

        foreach (var hashtag in existing)
        {
            hashtag.UsageCount += 1;
        }

        var now = DateTime.UtcNow;
        var created = normalizedTags
            .Where(tag => !existingTags.Contains(tag))
            .Select(tag => new Hashtag
            {
                Tag = tag,
                UsageCount = 1,
                CreatedAt = now
            })
            .ToList();

        if (created.Count > 0)
        {
            await _dbContext.Hashtags.AddRangeAsync(created, ct);
        }

        await _dbContext.SaveChangesAsync(ct);

        return existing.Concat(created).ToList();
    }

    public async Task<IReadOnlyList<Hashtag>> SearchAsync(
        string query,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<Hashtag>();
        }

        var term = query.Trim();
        if (!term.StartsWith('#'))
        {
            term = $"#{term}";
        }

        term = term.ToLowerInvariant();

        return await _dbContext.Hashtags
            .AsNoTracking()
            .Include(hashtag => hashtag.PostHashtags)
                .ThenInclude(postHashtag => postHashtag.Post)
                    .ThenInclude(post => post.User)
            .Where(hashtag => EF.Functions.Like(hashtag.Tag, $"{term}%"))
            .OrderByDescending(hashtag => hashtag.UsageCount)
            .ApplyPaging(pageNumber, pageSize, defaultPageSize: 20)
            .ToListAsync(ct);
    }

    public async Task AddPostHashtagsAsync(string postId, IEnumerable<string> hashtagIds, CancellationToken ct = default)
    {
        var ids = hashtagIds
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList();

        if (ids.Count == 0)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var postHashtags = ids
            .Select(id => new PostHashtag
            {
                PostId = postId,
                HashtagId = id,
                CreatedAt = now
            })
            .ToList();

        await _dbContext.PostHashtags.AddRangeAsync(postHashtags, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}
