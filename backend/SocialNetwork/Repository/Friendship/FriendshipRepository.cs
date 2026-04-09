using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Extensions;
using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public class FriendshipRepository : IFriendshipRepository
{
    private readonly ApplicationDbContext _dbContext;

    public FriendshipRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Friendship?> GetByIdAsync(string friendshipId, CancellationToken ct = default)
    {
        return _dbContext.Friendships
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.FriendshipId == friendshipId, ct);
    }

    public Task<Friendship?> GetBetweenUsersAsync(string userId1, string userId2, CancellationToken ct = default)
    {
        return _dbContext.Friendships
            .AsNoTracking()
            .FirstOrDefaultAsync(
                f => (f.UserId1 == userId1 && f.UserId2 == userId2) ||
                     (f.UserId1 == userId2 && f.UserId2 == userId1),
                ct);
    }

    public Task<bool> ExistsBetweenUsersAsync(string userId1, string userId2, CancellationToken ct = default)
    {
        return _dbContext.Friendships.AnyAsync(
            f => (f.UserId1 == userId1 && f.UserId2 == userId2) ||
                 (f.UserId1 == userId2 && f.UserId2 == userId1),
            ct);
    }

    public async Task<IReadOnlyList<Friendship>> GetPendingRequestsForUserAsync(
        string userId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var friendships = await _dbContext.Friendships
            .AsNoTracking()
            .Where(f => f.UserId2 == userId && f.Status == "Pending")
            .OrderByDescending(f => f.CreatedAt)
            .ApplyPaging(pageNumber, pageSize, defaultPageSize: 50)
            .ToListAsync(ct);

        return friendships;
    }

    public async Task<IReadOnlyList<Friendship>> GetAcceptedFriendshipsForUserAsync(
        string userId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var friendships = await _dbContext.Friendships
            .AsNoTracking()
            .Where(f => f.Status == "Accepted" && (f.UserId1 == userId || f.UserId2 == userId))
            .OrderByDescending(f => f.UpdatedAt ?? f.CreatedAt)
            .ApplyPaging(pageNumber, pageSize, defaultPageSize: 50)
            .ToListAsync(ct);

        return friendships;
    }

    public async Task AddAsync(Friendship friendship, CancellationToken ct = default)
    {
        await _dbContext.Friendships.AddAsync(friendship, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> UpdateStatusAsync(string friendshipId, string status, CancellationToken ct = default)
    {
        var affectedRows = await _dbContext.Friendships
            .Where(f => f.FriendshipId == friendshipId)
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(f => f.Status, status)
                    .SetProperty(f => f.UpdatedAt, DateTime.UtcNow),
                ct);

        return affectedRows > 0;
    }

    public async Task DeleteAsync(string friendshipId, CancellationToken ct = default)
    {
        await _dbContext.Friendships
            .Where(f => f.FriendshipId == friendshipId)
            .ExecuteDeleteAsync(ct);
    }
}
