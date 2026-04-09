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

    public async Task AddAsync(Like like, CancellationToken ct = default)
    {
        await _dbContext.Likes.AddAsync(like, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}
