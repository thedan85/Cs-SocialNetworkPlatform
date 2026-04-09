using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Extensions;
using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CommentRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Comment>> GetByPostIdAsync(
        string postId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var comments = await _dbContext.Comments
            .AsNoTracking()
            .Where(comment => comment.PostId == postId)
            .OrderByDescending(comment => comment.CreatedAt)
            .ApplyPaging(pageNumber, pageSize)
            .ToListAsync(ct);

        return comments;
    }

    public async Task AddAsync(Comment comment, CancellationToken ct = default)
    {
        await _dbContext.Comments.AddAsync(comment, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> DeleteAsync(string postId, string commentId, CancellationToken ct = default)
    {
        var affectedRows = await _dbContext.Comments
            .Where(entity => entity.PostId == postId && entity.CommentId == commentId)
            .ExecuteDeleteAsync(ct);

        return affectedRows > 0;
    }
}