using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Dtos;
using SocialNetwork.Extensions;
using SocialNetwork.Model;
using SocialNetwork.Repository;

namespace SocialNetwork.Service;

public class UsersService : IUsersService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;

    public UsersService(
        ApplicationDbContext dbContext,
        UserManager<User> userManager,
        IUserRepository userRepository)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _userRepository = userRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<UserResponse>>> GetUsersAsync(CancellationToken ct = default)
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.UserName)
            .Select(user => user.ToUserResponse())
            .ToListAsync(ct);

        return ServiceResult<IReadOnlyList<UserResponse>>.Ok(users);
    }

    public async Task<ServiceResult<UserResponse>> GetUserByIdAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user is null)
        {
            return ServiceResult<UserResponse>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        return ServiceResult<UserResponse>.Ok(user.ToUserResponse());
    }

    public async Task<ServiceResult<UserResponse>> UpdateUserAsync(
        string userId,
        UserUpdateRequest request,
        CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return ServiceResult<UserResponse>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        if (request.ProfilePicture is not null)
        {
            user.ProfilePicture = request.ProfilePicture;
        }

        if (request.Bio is not null)
        {
            user.Bio = request.Bio;
        }

        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        user.UpdatedAt = DateTime.UtcNow;

        var identityResult = await _userManager.UpdateAsync(user);
        if (!identityResult.Succeeded)
        {
            return ServiceResult<UserResponse>.Fail(
                ServiceErrorType.Validation,
                identityResult.ToErrorMessages());
        }

        return ServiceResult<UserResponse>.Ok(user.ToUserResponse());
    }

    public async Task<ServiceResult<IReadOnlyList<PostResponse>>> GetUserPostsAsync(
        string userId,
        CancellationToken ct = default)
    {
        var userExists = await _userRepository.ExistsByIdAsync(userId, ct);
        if (!userExists)
        {
            return ServiceResult<IReadOnlyList<PostResponse>>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var posts = await _dbContext.Posts
            .AsNoTracking()
            .Where(post => post.UserId == userId)
            .OrderByDescending(post => post.CreatedAt)
            .Select(post => post.ToPostResponse())
            .ToListAsync(ct);

        return ServiceResult<IReadOnlyList<PostResponse>>.Ok(posts);
    }
}
