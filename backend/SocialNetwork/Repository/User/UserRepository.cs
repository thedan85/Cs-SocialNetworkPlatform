using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<User>> GetAllOrderedByUserNameAsync(CancellationToken ct = default)
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .ToListAsync(ct);

        return users;
    }

    public Task<User?> GetByIdAsync(string userId, CancellationToken ct = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
    }

    public Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName, ct);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public Task<bool> ExistsByIdAsync(string userId, CancellationToken ct = default)
    {
        return _dbContext.Users.AnyAsync(u => u.Id == userId, ct);
    }

    public Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default)
    {
        return _dbContext.Users.AnyAsync(u => u.UserName == userName, ct);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
    {
        return _dbContext.Users.AnyAsync(u => u.Email == email, ct);
    }

    public async Task UpdateProfileAsync(
        string userId,
        string? bio,
        string? profilePicture,
        CancellationToken ct = default)
    {
        var affectedRows = await _dbContext.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(u => u.Bio, bio)
                    .SetProperty(u => u.ProfilePicture, profilePicture)
                    .SetProperty(u => u.UpdatedAt, DateTime.UtcNow),
                ct);

        if (affectedRows == 0)
        {
            throw new KeyNotFoundException($"User with id '{userId}' was not found.");
        }
    }

    public async Task SetActiveStatusAsync(string userId, bool isActive, CancellationToken ct = default)
    {
        var affectedRows = await _dbContext.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(
                setter => setter
                    .SetProperty(u => u.IsActive, isActive)
                    .SetProperty(u => u.UpdatedAt, DateTime.UtcNow),
                ct);

        if (affectedRows == 0)
        {
            throw new KeyNotFoundException($"User with id '{userId}' was not found.");
        }
    }
}
