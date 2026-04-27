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
            .Include(post => post.User)
            .Include(post => post.SharedPost)
            .ThenInclude(shared => shared.User)
            .OrderByDescending(post => post.CreatedAt)
            .ApplyPaging(pageNumber, pageSize)
            .ToListAsync(ct);

        return posts;
    }

    public async Task<IReadOnlyList<Post>> GetVisiblePagedAsync(
        string viewerUserId,
        bool isAdmin,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _dbContext.Posts
            .AsNoTracking()
            .Include(post => post.User)
            .Include(post => post.SharedPost)
            .ThenInclude(shared => shared.User)
            .OrderByDescending(post => post.CreatedAt)
            .AsQueryable();

        if (!isAdmin)
        {
            var friendIds = _dbContext.Friendships
                .Where(friendship =>
                    friendship.Status == "Accepted" &&
                    (friendship.UserId1 == viewerUserId || friendship.UserId2 == viewerUserId))
                .Select(friendship =>
                    friendship.UserId1 == viewerUserId ? friendship.UserId2 : friendship.UserId1);

            query = query.Where(post =>
                post.UserId == viewerUserId ||
                post.Privacy == null ||
                post.Privacy == PostPrivacy.Public ||
                (post.Privacy == PostPrivacy.Friends && friendIds.Contains(post.UserId)));
        }

        return await query
            .ApplyPaging(pageNumber, pageSize)
            .ToListAsync(ct);
    }

    public Task<Post?> GetByIdAsync(string postId, CancellationToken ct = default)
    {
        return _dbContext.Posts
            .AsNoTracking()
            .Include(post => post.User)
            .Include(post => post.SharedPost)
            .ThenInclude(shared => shared.User)
            .FirstOrDefaultAsync(p => p.PostId == postId, ct);
    }

    public Task<Post?> GetSharePostByUserAsync(string sharedPostId, string userId, CancellationToken ct = default)
    {
        return _dbContext.Posts
            .AsNoTracking()
            .Include(post => post.User)
            .Include(post => post.SharedPost)
            .ThenInclude(shared => shared.User)
            .OrderByDescending(post => post.CreatedAt)
            .FirstOrDefaultAsync(
                post => post.SharedPostId == sharedPostId && post.UserId == userId,
                ct);
    }

    public Task<bool> ExistsByIdAsync(string postId, CancellationToken ct = default)
    {
        return _dbContext.Posts.AnyAsync(p => p.PostId == postId, ct);
    }

    public async Task<IReadOnlyList<Post>> GetByUserIdOrderedAsync(string userId, CancellationToken ct = default)
    {
        var posts = await _dbContext.Posts
            .AsNoTracking()
            .Include(post => post.User)
            .Include(post => post.SharedPost)
            .ThenInclude(shared => shared.User)
            .Where(post => post.UserId == userId)
            .OrderByDescending(post => post.CreatedAt)
            .ToListAsync(ct);

        return posts;
    }

    public async Task<IReadOnlyList<Post>> GetByUserIdWithPrivacyAsync(
        string userId,
        IReadOnlyList<string> privacyValues,
        CancellationToken ct = default)
    {
        var posts = await _dbContext.Posts
            .AsNoTracking()
            .Include(post => post.User)
            .Include(post => post.SharedPost)
            .ThenInclude(shared => shared.User)
            .Where(post => post.UserId == userId && privacyValues.Contains(post.Privacy ?? PostPrivacy.Public))
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
            .Include(post => post.User)
            .Include(post => post.SharedPost)
            .ThenInclude(shared => shared.User)
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

    public async Task<bool> IncrementLikeCountAsync(string postId, int delta, CancellationToken ct = default)
    {
        var affectedRows = await _dbContext.Posts
            .Where(p => p.PostId == postId)
            .ExecuteUpdateAsync(
                setter => setter.SetProperty(p => p.LikeCount, p => p.LikeCount + delta),
                ct);

        return affectedRows > 0;
    }

    public async Task AddAsync(Post post, CancellationToken ct = default)
    {
        // Avoid inserting an existing User when the navigation is attached.
        post.User = null;
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
                    .SetProperty(p => p.Privacy, post.Privacy)
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
