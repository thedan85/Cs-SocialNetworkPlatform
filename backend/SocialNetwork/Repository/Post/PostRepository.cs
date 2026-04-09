using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Extensions;
using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PostRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Post>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var posts = await _dbContext.Posts
            .AsNoTracking()
            .OrderByDescending(post => post.CreatedAt)
            .ApplyPaging(pageNumber, pageSize)
            .ToListAsync(ct);

        return posts;
    }

    public Task<Post?> GetByIdAsync(string postId, CancellationToken ct = default)
    {
        return _dbContext.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PostId == postId, ct);
    }

    public async Task<IReadOnlyList<Post>> GetByUserIdOrderedAsync(string userId, CancellationToken ct = default)
    {
        var posts = await _dbContext.Posts
            .AsNoTracking()
            .Where(post => post.UserId == userId)
            .OrderByDescending(post => post.CreatedAt)
            .ToListAsync(ct);

        return posts;
    }

    public async Task<IReadOnlyList<Post>> GetByUserIdAsync(
        string userId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var posts = await _dbContext.Posts
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ApplyPaging(pageNumber, pageSize)
            .ToListAsync(ct);

        return posts;
    }

    public Task<int> CountByUserIdAsync(string userId, CancellationToken ct = default)
    {
        return _dbContext.Posts.CountAsync(p => p.UserId == userId, ct);
    }

    public async Task AddAsync(Post post, CancellationToken ct = default)
    {
        await _dbContext.Posts.AddAsync(post, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Post post, CancellationToken ct = default)
    {
        var updatedAt = post.UpdatedAt == default
            ? DateTime.UtcNow
            : post.UpdatedAt;

        var affectedRows = await _dbContext.Posts
            .Where(p => p.PostId == post.PostId)
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(p => p.Content, post.Content)
                    .SetProperty(p => p.ImageUrl, post.ImageUrl)
                    .SetProperty(p => p.UpdatedAt, updatedAt),
                ct);

        if (affectedRows == 0)
        {
            throw new KeyNotFoundException($"Post with id '{post.PostId}' was not found.");
        }
    }

    public async Task DeleteAsync(string postId, CancellationToken ct = default)
    {
        await _dbContext.Posts
            .Where(p => p.PostId == postId)
            .ExecuteDeleteAsync(ct);
    }
}
