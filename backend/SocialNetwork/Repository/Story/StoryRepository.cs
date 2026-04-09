using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Extensions;
using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public class StoryRepository : IStoryRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StoryRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Story>> GetActiveAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var stories = await _dbContext.Stories
            .AsNoTracking()
            .Where(story => story.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(story => story.CreatedAt)
            .ApplyPaging(pageNumber, pageSize, defaultPageSize: 50)
            .ToListAsync(ct);

        return stories;
    }

    public Task<Story?> GetByIdAsync(string storyId, CancellationToken ct = default)
    {
        return _dbContext.Stories
            .AsNoTracking()
            .FirstOrDefaultAsync(story => story.StoryId == storyId, ct);
    }

    public async Task<IReadOnlyList<Story>> GetActiveByUserIdAsync(
        string userId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var stories = await _dbContext.Stories
            .AsNoTracking()
            .Where(story => story.UserId == userId && story.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(story => story.CreatedAt)
            .ApplyPaging(pageNumber, pageSize, defaultPageSize: 50)
            .ToListAsync(ct);

        return stories;
    }

    public async Task AddAsync(Story story, CancellationToken ct = default)
    {
        await _dbContext.Stories.AddAsync(story, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteAsync(string storyId, CancellationToken ct = default)
    {
        var affectedRows = await _dbContext.Stories
            .Where(entity => entity.StoryId == storyId)
            .ExecuteDeleteAsync(ct);

        return affectedRows > 0;
    }
}
