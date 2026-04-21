using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public class PostShareRepository : IPostShareRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PostShareRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<PostShare?> GetByPostAndUserAsync(string postId, string userId, CancellationToken ct = default)
    {
        return _dbContext.PostShares
            .AsNoTracking()
            .FirstOrDefaultAsync(share => share.PostId == postId && share.UserId == userId, ct);
    }

    public async Task<IReadOnlyList<string>> GetSharedPostIdsAsync(
        string userId,
        IReadOnlyCollection<string> postIds,
        CancellationToken ct = default)
    {
        if (postIds.Count == 0)
        {
            return Array.Empty<string>();
        }

        return await _dbContext.PostShares
            .AsNoTracking()
            .Where(share => share.UserId == userId && postIds.Contains(share.PostId))
            .Select(share => share.PostId)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyDictionary<string, int>> GetShareCountsAsync(
        IReadOnlyCollection<string> postIds,
        CancellationToken ct = default)
    {
        if (postIds.Count == 0)
        {
            return new Dictionary<string, int>();
        }

        return await _dbContext.PostShares
            .AsNoTracking()
            .Where(share => postIds.Contains(share.PostId))
            .GroupBy(share => share.PostId)
            .Select(group => new { group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.Key, item => item.Count, ct);
    }

    public Task<int> CountByPostIdAsync(string postId, CancellationToken ct = default)
    {
        return _dbContext.PostShares.CountAsync(share => share.PostId == postId, ct);
    }

    public async Task AddAsync(PostShare share, CancellationToken ct = default)
    {
        await _dbContext.PostShares.AddAsync(share, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteByPostAndUserAsync(string postId, string userId, CancellationToken ct = default)
    {
        var affectedRows = await _dbContext.PostShares
            .Where(share => share.PostId == postId && share.UserId == userId)
            .ExecuteDeleteAsync(ct);

        return affectedRows > 0;
    }
}
