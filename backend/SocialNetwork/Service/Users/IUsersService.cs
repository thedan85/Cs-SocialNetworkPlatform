using SocialNetwork.Dtos;

namespace SocialNetwork.Service;

public interface IUsersService
{
    Task<ServiceResult<IReadOnlyList<UserResponse>>> GetUsersAsync(CancellationToken ct = default);
    Task<ServiceResult<UserResponse>> GetUserByIdAsync(
        string actorUserId,
        string userId,
        bool isAdmin,
        CancellationToken ct = default);
    Task<ServiceResult<IReadOnlyList<UserResponse>>> SearchUsersAsync(
        string query,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default);
    Task<ServiceResult<UserResponse>> UpdateUserAsync(string userId, UserUpdateRequest request, CancellationToken ct = default);
    Task<ServiceResult<IReadOnlyList<PostResponse>>> GetUserPostsAsync(
        string actorUserId,
        string userId,
        bool isAdmin,
        CancellationToken ct = default);
}
