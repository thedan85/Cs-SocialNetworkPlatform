using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
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
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
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
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
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
        var friendship = await _dbContext.Friendships.FirstOrDefaultAsync(f => f.FriendshipId == friendshipId, ct);
        if (friendship is null)
        {
            return false;
        }

        friendship.Status = status;
        friendship.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task DeleteAsync(string friendshipId, CancellationToken ct = default)
    {
        var friendship = await _dbContext.Friendships.FirstOrDefaultAsync(f => f.FriendshipId == friendshipId, ct);
        if (friendship is null)
        {
            return;
        }

        _dbContext.Friendships.Remove(friendship);
        await _dbContext.SaveChangesAsync(ct);
    }
}
