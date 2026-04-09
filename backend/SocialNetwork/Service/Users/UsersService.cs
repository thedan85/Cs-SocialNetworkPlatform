using Microsoft.AspNetCore.Identity;
using SocialNetwork.Dtos;
using SocialNetwork.Extensions;
using SocialNetwork.Model;
using SocialNetwork.Repository;

namespace SocialNetwork.Service;

public class UsersService : IUsersService
{
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly IPostRepository _postRepository;

    public UsersService(
        UserManager<User> userManager,
        IUserRepository userRepository,
        IPostRepository postRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _postRepository = postRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<UserResponse>>> GetUsersAsync(CancellationToken ct = default)
    {
        var users = await _userRepository.GetAllOrderedByUserNameAsync(ct);

        var responses = users
            .Select(user => user.ToUserResponse())
            .ToList();

        return ServiceResult<IReadOnlyList<UserResponse>>.Ok(responses);
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

        var posts = await _postRepository.GetByUserIdOrderedAsync(userId, ct);

        var responses = posts
            .Select(post => post.ToPostResponse())
            .ToList();

        return ServiceResult<IReadOnlyList<PostResponse>>.Ok(responses);
    }
}
