using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public class PostReportRepository : IPostReportRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PostReportRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<PostReport?> GetByIdAsync(string postReportId, CancellationToken ct = default)
    {
        return _dbContext.PostReports
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.PostReportId == postReportId, ct);
    }

    public async Task<IReadOnlyList<PostReport>> GetByPostIdAsync(
        string postId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var reports = await _dbContext.PostReports
            .AsNoTracking()
            .Where(r => r.PostId == postId)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return reports;
    }

    public async Task<IReadOnlyList<PostReport>> GetPendingAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var reports = await _dbContext.PostReports
            .AsNoTracking()
            .Where(r => !r.Status)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return reports;
    }

    public Task<int> CountPendingAsync(CancellationToken ct = default)
    {
        return _dbContext.PostReports.CountAsync(r => !r.Status, ct);
    }

    public Task<bool> ExistsByPostAndReporterAsync(string postId, string reporterUserId, CancellationToken ct = default)
    {
        return _dbContext.PostReports.AnyAsync(
            r => r.PostId == postId && r.ReporterUserId == reporterUserId,
            ct);
    }

    public async Task AddAsync(PostReport postReport, CancellationToken ct = default)
    {
        await _dbContext.PostReports.AddAsync(postReport, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> SetReviewStatusAsync(string postReportId, bool reviewed, CancellationToken ct = default)
    {
        var postReport = await _dbContext.PostReports
            .FirstOrDefaultAsync(r => r.PostReportId == postReportId, ct);
        if (postReport is null)
        {
            return false;
        }

        postReport.Status = reviewed;
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }
}
