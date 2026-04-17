using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Extensions;
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

    public async Task<IReadOnlyList<User>> SearchByNameAsync(
        string query,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Array.Empty<User>();
        }

        var tokens = query
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var users = _dbContext.Users.AsNoTracking();

        if (tokens.Length == 1)
        {
            var term = tokens[0];
            users = users.Where(u =>
                (u.FirstName != null && EF.Functions.Like(u.FirstName, $"{term}%")) ||
                (u.LastName != null && EF.Functions.Like(u.LastName, $"{term}%")) ||
                (u.UserName != null && EF.Functions.Like(u.UserName, $"{term}%")));
        }
        else
        {
            var first = tokens[0];
            var last = string.Join(' ', tokens.Skip(1));
            var fullQuery = string.Join(' ', tokens);

            users = users.Where(u =>
                (u.FirstName != null && u.LastName != null &&
                 EF.Functions.Like(u.FirstName, $"{first}%") &&
                 EF.Functions.Like(u.LastName, $"{last}%")) ||
                (u.UserName != null && EF.Functions.Like(u.UserName, $"{fullQuery}%")));
        }

        return await users
            .OrderBy(u => u.UserName)
            .ApplyPaging(pageNumber, pageSize, defaultPageSize: 50)
            .ToListAsync(ct);
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
