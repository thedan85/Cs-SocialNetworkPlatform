using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Dtos;
using SocialNetwork.Extensions;
using SocialNetwork.Model;
using SocialNetwork.Repository;

namespace SocialNetwork.Service;

public class PostsService : IPostsService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPostRepository _postRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPostReportRepository _postReportRepository;

    public PostsService(
        ApplicationDbContext dbContext,
        IPostRepository postRepository,
        IUserRepository userRepository,
        IPostReportRepository postReportRepository)
    {
        _dbContext = dbContext;
        _postRepository = postRepository;
        _userRepository = userRepository;
        _postReportRepository = postReportRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<PostResponse>>> GetPostsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var posts = await _dbContext.Posts
            .AsNoTracking()
            .OrderByDescending(post => post.CreatedAt)
            .ApplyPaging(pageNumber, pageSize)
            .Select(post => post.ToPostResponse())
            .ToListAsync(ct);

        return ServiceResult<IReadOnlyList<PostResponse>>.Ok(posts);
    }

    public async Task<ServiceResult<PostResponse>> GetPostByIdAsync(string postId, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post is null)
        {
            return ServiceResult<PostResponse>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        return ServiceResult<PostResponse>.Ok(post.ToPostResponse());
    }

    public async Task<ServiceResult<PostResponse>> CreatePostAsync(PostCreateRequest request, CancellationToken ct = default)
    {
        var userExists = await _userRepository.ExistsByIdAsync(request.UserId, ct);
        if (!userExists)
        {
            return ServiceResult<PostResponse>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var post = new Post
        {
            UserId = request.UserId,
            Content = request.Content,
            ImageUrl = request.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _postRepository.AddAsync(post, ct);
        return ServiceResult<PostResponse>.Ok(post.ToPostResponse());
    }

    public async Task<ServiceResult<PostResponse>> UpdatePostAsync(
        string postId,
        PostUpdateRequest request,
        CancellationToken ct = default)
    {
        var existingPost = await _postRepository.GetByIdAsync(postId, ct);
        if (existingPost is null)
        {
            return ServiceResult<PostResponse>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        existingPost.Content = request.Content;
        existingPost.ImageUrl = request.ImageUrl;

        await _postRepository.UpdateAsync(existingPost, ct);

        var updatedPost = await _postRepository.GetByIdAsync(postId, ct);
        if (updatedPost is null)
        {
            return ServiceResult<PostResponse>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        return ServiceResult<PostResponse>.Ok(updatedPost.ToPostResponse());
    }

    public async Task<ServiceResult<string>> DeletePostAsync(string postId, CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post is null)
        {
            return ServiceResult<string>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        await _postRepository.DeleteAsync(postId, ct);
        return ServiceResult<string>.Ok("Post deleted.");
    }

    public async Task<ServiceResult<IReadOnlyList<CommentResponse>>> GetPostCommentsAsync(
        string postId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post is null)
        {
            return ServiceResult<IReadOnlyList<CommentResponse>>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        var comments = await _dbContext.Comments
            .AsNoTracking()
            .Where(comment => comment.PostId == postId)
            .OrderByDescending(comment => comment.CreatedAt)
            .ApplyPaging(pageNumber, pageSize, defaultPageSize: 50)
            .Select(comment => comment.ToCommentResponse())
            .ToListAsync(ct);

        return ServiceResult<IReadOnlyList<CommentResponse>>.Ok(comments);
    }

    public async Task<ServiceResult<CommentResponse>> CreateCommentAsync(
        string postId,
        CommentCreateRequest request,
        CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post is null)
        {
            return ServiceResult<CommentResponse>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        var userExists = await _userRepository.ExistsByIdAsync(request.UserId, ct);
        if (!userExists)
        {
            return ServiceResult<CommentResponse>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var comment = new Comment
        {
            PostId = postId,
            UserId = request.UserId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.Comments.AddAsync(comment, ct);
        await _dbContext.SaveChangesAsync(ct);

        return ServiceResult<CommentResponse>.Ok(comment.ToCommentResponse());
    }

    public async Task<ServiceResult<string>> DeleteCommentAsync(
        string postId,
        string commentId,
        CancellationToken ct = default)
    {
        var deletedRows = await _dbContext.Comments
            .Where(entity => entity.PostId == postId && entity.CommentId == commentId)
            .ExecuteDeleteAsync(ct);

        if (deletedRows == 0)
        {
            return ServiceResult<string>.Fail(ServiceErrorType.NotFound, "Comment not found.");
        }

        return ServiceResult<string>.Ok("Comment deleted.");
    }

    public async Task<ServiceResult<LikePostResult>> LikePostAsync(
        string postId,
        LikeCreateRequest request,
        CancellationToken ct = default)
    {
        var postExists = await _dbContext.Posts
            .AsNoTracking()
            .AnyAsync(entity => entity.PostId == postId, ct);

        if (!postExists)
        {
            return ServiceResult<LikePostResult>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        var userExists = await _userRepository.ExistsByIdAsync(request.UserId, ct);
        if (!userExists)
        {
            return ServiceResult<LikePostResult>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var existingLike = await _dbContext.Likes
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.PostId == postId && entity.UserId == request.UserId, ct);

        if (existingLike is not null)
        {
            return ServiceResult<LikePostResult>.Ok(new LikePostResult
            {
                Like = existingLike.ToLikeResponse(),
                IsCreated = false
            });
        }

        var like = new Like
        {
            PostId = postId,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

        await _dbContext.Likes.AddAsync(like, ct);
        await _dbContext.SaveChangesAsync(ct);

        await _dbContext.Posts
            .Where(p => p.PostId == postId)
            .ExecuteUpdateAsync(
                setter => setter.SetProperty(p => p.LikeCount, p => p.LikeCount + 1),
                ct);

        await transaction.CommitAsync(ct);

        return ServiceResult<LikePostResult>.Ok(new LikePostResult
        {
            Like = like.ToLikeResponse(),
            IsCreated = true
        });
    }

    public async Task<ServiceResult<PostReportResponse>> ReportPostAsync(
        string postId,
        PostReportCreateRequest request,
        CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post is null)
        {
            return ServiceResult<PostReportResponse>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        var userExists = await _userRepository.ExistsByIdAsync(request.ReporterUserId, ct);
        if (!userExists)
        {
            return ServiceResult<PostReportResponse>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var existingReport = await _postReportRepository.ExistsByPostAndReporterAsync(
            postId,
            request.ReporterUserId,
            ct);

        if (existingReport)
        {
            return ServiceResult<PostReportResponse>.Fail(
                ServiceErrorType.Conflict,
                "You have already reported this post.");
        }

        var postReport = new PostReport
        {
            PostId = postId,
            ReporterUserId = request.ReporterUserId,
            Reason = request.Reason,
            Description = request.Description,
            Status = false,
            CreatedAt = DateTime.UtcNow
        };

        await _postReportRepository.AddAsync(postReport, ct);

        return ServiceResult<PostReportResponse>.Ok(postReport.ToPostReportResponse());
    }
}
