using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetAllOrderedByUserNameAsync(CancellationToken ct = default);

    Task<User?> GetByIdAsync(string userId, CancellationToken ct = default);
    Task<User?> GetByUserNameAsync(string userName, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    Task<IReadOnlyList<User>> SearchByNameAsync(
        string query,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<bool> ExistsByIdAsync(string userId, CancellationToken ct = default);
    Task<bool> ExistsByUserNameAsync(string userName, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);

    Task UpdateProfileAsync(
        string userId,
        string? bio,
        string? profilePicture,
        CancellationToken ct = default);

    Task SetActiveStatusAsync(string userId, bool isActive, CancellationToken ct = default);
}