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
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly IPostShareRepository _postShareRepository;

    public UsersService(
        UserManager<User> userManager,
        IUserRepository userRepository,
        IPostRepository postRepository,
        IFriendshipRepository friendshipRepository,
        ILikeRepository likeRepository,
        IPostShareRepository postShareRepository)
    {
        _userManager = userManager;
        _userRepository = userRepository;
        _postRepository = postRepository;
        _friendshipRepository = friendshipRepository;
        _likeRepository = likeRepository;
        _postShareRepository = postShareRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<UserResponse>>> GetUsersAsync(CancellationToken ct = default)
    {
        var users = await _userRepository.GetAllOrderedByUserNameAsync(ct);

        var responses = users
            .Select(user => user.ToUserResponse())
            .ToList();

        return ServiceResult<IReadOnlyList<UserResponse>>.Ok(responses);
    }

    public async Task<ServiceResult<UserResponse>> GetUserByIdAsync(
        string actorUserId,
        string userId,
        bool isAdmin,
        CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, ct);
        if (user is null)
        {
            return ServiceResult<UserResponse>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var response = user.ToUserResponse();
        if (!isAdmin && !string.Equals(actorUserId, userId, StringComparison.OrdinalIgnoreCase))
        {
            response.Email = string.Empty;
        }

        return ServiceResult<UserResponse>.Ok(response);
    }

    public async Task<ServiceResult<IReadOnlyList<UserResponse>>> SearchUsersAsync(
        string query,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        var users = await _userRepository.SearchByNameAsync(query, pageNumber, pageSize, ct);

        var responses = users
            .Select(user => user.ToUserResponse())
            .ToList();

        return ServiceResult<IReadOnlyList<UserResponse>>.Ok(responses);
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

        if (request.FirstName is not null)
        {
            user.FirstName = request.FirstName;
        }

        if (request.LastName is not null)
        {
            user.LastName = request.LastName;
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
        string actorUserId,
        string userId,
        bool isAdmin,
        CancellationToken ct = default)
    {
        var userExists = await _userRepository.ExistsByIdAsync(userId, ct);
        if (!userExists)
        {
            return ServiceResult<IReadOnlyList<PostResponse>>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        IReadOnlyList<Post> posts;

        if (isAdmin || string.Equals(actorUserId, userId, StringComparison.OrdinalIgnoreCase))
        {
            posts = await _postRepository.GetByUserIdOrderedAsync(userId, ct);
        }
        else
        {
            var areFriends = await _friendshipRepository.AreFriendsAsync(actorUserId, userId, ct);
            var allowedPrivacy = areFriends
                ? new[] { PostPrivacy.Public, PostPrivacy.Friends }
                : new[] { PostPrivacy.Public };
            posts = await _postRepository.GetByUserIdWithPrivacyAsync(userId, allowedPrivacy, ct);
        }

        var postIds = posts.Select(post => post.PostId).ToList();
        var likedPostIds = await _likeRepository.GetLikedPostIdsAsync(actorUserId, postIds, ct);
        var sharedPostIds = await _postShareRepository.GetSharedPostIdsAsync(actorUserId, postIds, ct);
        var shareCounts = await _postShareRepository.GetShareCountsAsync(postIds, ct);
        var likedPostIdSet = new HashSet<string>(likedPostIds, StringComparer.OrdinalIgnoreCase);
        var sharedPostIdSet = new HashSet<string>(sharedPostIds, StringComparer.OrdinalIgnoreCase);

        var responses = posts
            .Select(post =>
            {
                var response = post.ToPostResponse();
                response.IsLiked = likedPostIdSet.Contains(post.PostId);
                response.IsShared = sharedPostIdSet.Contains(post.PostId);
                response.ShareCount = shareCounts.TryGetValue(post.PostId, out var count) ? count : 0;
                return response;
            })
            .ToList();

        return ServiceResult<IReadOnlyList<PostResponse>>.Ok(responses);
    }
}
