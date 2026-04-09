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
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null)
        {
            throw new KeyNotFoundException($"User with id '{userId}' was not found.");
        }

        user.Bio = bio;
        user.ProfilePicture = profilePicture;
        user.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task SetActiveStatusAsync(string userId, bool isActive, CancellationToken ct = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null)
        {
            throw new KeyNotFoundException($"User with id '{userId}' was not found.");
        }

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(ct);
    }
}
