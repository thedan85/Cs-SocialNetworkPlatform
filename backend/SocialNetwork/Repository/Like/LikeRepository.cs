using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public class LikeRepository : ILikeRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LikeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Like?> GetByPostAndUserAsync(string postId, string userId, CancellationToken ct = default)
    {
        return _dbContext.Likes
            .AsNoTracking()
            .FirstOrDefaultAsync(like => like.PostId == postId && like.UserId == userId, ct);
    }

    public async Task<IReadOnlyList<string>> GetLikedPostIdsAsync(
        string userId,
        IReadOnlyCollection<string> postIds,
        CancellationToken ct = default)
    {
        if (postIds.Count == 0)
        {
            return Array.Empty<string>();
        }

        return await _dbContext.Likes
            .AsNoTracking()
            .Where(like => like.UserId == userId && like.PostId != null && postIds.Contains(like.PostId))
            .Select(like => like.PostId!)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Like like, CancellationToken ct = default)
    {
        await _dbContext.Likes.AddAsync(like, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteByPostAndUserAsync(string postId, string userId, CancellationToken ct = default)
    {
        var affectedRows = await _dbContext.Likes
            .Where(like => like.PostId == postId && like.UserId == userId)
            .ExecuteDeleteAsync(ct);

        return affectedRows > 0;
    }
}
