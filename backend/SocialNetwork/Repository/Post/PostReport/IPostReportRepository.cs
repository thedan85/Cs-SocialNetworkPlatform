using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public interface IPostReportRepository
{
    Task<PostReport?> GetByIdAsync(string postReportId, CancellationToken ct = default);

    Task<IReadOnlyList<PostReport>> GetByPostIdAsync(
        string postId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<IReadOnlyList<PostReport>> GetPendingAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<int> CountPendingAsync(CancellationToken ct = default);

    Task<bool> ExistsByPostAndReporterAsync(string postId, string reporterUserId, CancellationToken ct = default);

    Task AddAsync(PostReport postReport, CancellationToken ct = default);

    Task<bool> SetReviewStatusAsync(string postReportId, bool reviewed, CancellationToken ct = default);
}
