using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PostRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Post?> GetByIdAsync(string postId, CancellationToken ct = default)
    {
        return _dbContext.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PostId == postId, ct);
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
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
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
        var existingPost = await _dbContext.Posts.FirstOrDefaultAsync(p => p.PostId == post.PostId, ct);
        if (existingPost is null)
        {
            throw new KeyNotFoundException($"Post with id '{post.PostId}' was not found.");
        }

        existingPost.Content = post.Content;
        existingPost.ImageUrl = post.ImageUrl;
        existingPost.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(string postId, CancellationToken ct = default)
    {
        var existingPost = await _dbContext.Posts.FirstOrDefaultAsync(p => p.PostId == postId, ct);
        if (existingPost is null)
        {
            return;
        }

        _dbContext.Posts.Remove(existingPost);
        await _dbContext.SaveChangesAsync(ct);
    }
}
