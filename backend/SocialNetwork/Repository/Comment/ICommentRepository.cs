using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public interface ICommentRepository
{
    Task<IReadOnlyList<Comment>> GetByPostIdAsync(
        string postId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task AddAsync(Comment comment, CancellationToken ct = default);

    Task<bool> DeleteAsync(string postId, string commentId, CancellationToken ct = default);
}