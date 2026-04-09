using SocialNetwork.Dtos;

namespace SocialNetwork.Service;

public interface IUsersService
{
    Task<ServiceResult<IReadOnlyList<UserResponse>>> GetUsersAsync(CancellationToken ct = default);
    Task<ServiceResult<UserResponse>> GetUserByIdAsync(string userId, CancellationToken ct = default);
    Task<ServiceResult<UserResponse>> UpdateUserAsync(string userId, UserUpdateRequest request, CancellationToken ct = default);
    Task<ServiceResult<IReadOnlyList<PostResponse>>> GetUserPostsAsync(string userId, CancellationToken ct = default);
}
