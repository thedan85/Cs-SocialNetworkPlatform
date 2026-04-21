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
            .Include(f => f.User1)
            .Include(f => f.User2)
            .FirstOrDefaultAsync(f => f.FriendshipId == friendshipId, ct);
    }

    public Task<Friendship?> GetBetweenUsersAsync(string userId1, string userId2, CancellationToken ct = default)
    {
        return _dbContext.Friendships
            .AsNoTracking()
            .Include(f => f.User1)
            .Include(f => f.User2)
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

    public Task<bool> AreFriendsAsync(string userId1, string userId2, CancellationToken ct = default)
    {
        return _dbContext.Friendships.AnyAsync(
            f => f.Status == "Accepted" &&
                 ((f.UserId1 == userId1 && f.UserId2 == userId2) ||
                  (f.UserId1 == userId2 && f.UserId2 == userId1)),
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
            .Include(f => f.User1)
            .Include(f => f.User2)
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
            .Include(f => f.User1)
            .Include(f => f.User2)
            .Where(f => f.Status == "Accepted" && (f.UserId1 == userId || f.UserId2 == userId))
            .OrderByDescending(f => f.UpdatedAt ?? f.CreatedAt)
            .ApplyPaging(pageNumber, pageSize, defaultPageSize: 50)
            .ToListAsync(ct);

        return friendships;
    }

    public async Task<IReadOnlyList<string>> GetFriendIdsAsync(string userId, CancellationToken ct = default)
    {
        return await _dbContext.Friendships
            .AsNoTracking()
            .Where(f => f.Status == "Accepted" && (f.UserId1 == userId || f.UserId2 == userId))
            .Select(f => f.UserId1 == userId ? f.UserId2 : f.UserId1)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Friendship friendship, CancellationToken ct = default)
    {
        // Ensure we only persist the friendship row and avoid accidental inserts
        // of related User entities when they are attached to the graph.
        friendship.User1 = null;
        friendship.User2 = null;

        await _dbContext.Friendships.AddAsync(friendship, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> UpdateStatusAsync(
        string friendshipId,
        string status,
        DateTime updatedAt,
        CancellationToken ct = default)
    {
        var affectedRows = await _dbContext.Friendships
            .Where(f => f.FriendshipId == friendshipId)
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(f => f.Status, status)
                    .SetProperty(f => f.UpdatedAt, updatedAt),
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
