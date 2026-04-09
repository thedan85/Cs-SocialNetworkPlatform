using SocialNetwork.Dtos;
using SocialNetwork.Extensions;
using SocialNetwork.Model;
using SocialNetwork.Repository;

namespace SocialNetwork.Service;

public class PostsService : IPostsService
{
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPostReportRepository _postReportRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PostsService(
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        ILikeRepository likeRepository,
        IUserRepository userRepository,
        IPostReportRepository postReportRepository,
        IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _likeRepository = likeRepository;
        _userRepository = userRepository;
        _postReportRepository = postReportRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ServiceResult<IReadOnlyList<PostResponse>>> GetPostsAsync(
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var posts = await _postRepository.GetPagedAsync(pageNumber, pageSize, ct);

        var responses = posts
            .Select(post => post.ToPostResponse())
            .ToList();

        return ServiceResult<IReadOnlyList<PostResponse>>.Ok(responses);
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

    public async Task<ServiceResult<PostResponse>> CreatePostAsync(
        string actorUserId,
        PostCreateRequest request,
        CancellationToken ct = default)
    {
        var userExists = await _userRepository.ExistsByIdAsync(actorUserId, ct);
        if (!userExists)
        {
            return ServiceResult<PostResponse>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var post = new Post
        {
            UserId = actorUserId,
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

        var now = DateTime.UtcNow;

        existingPost.Content = request.Content;
        existingPost.ImageUrl = request.ImageUrl;
        existingPost.UpdatedAt = now;

        await _postRepository.UpdateAsync(existingPost, ct);
        return ServiceResult<PostResponse>.Ok(existingPost.ToPostResponse());
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
        int pageSize = 20,
        CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post is null)
        {
            return ServiceResult<IReadOnlyList<CommentResponse>>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        var comments = await _commentRepository.GetByPostIdAsync(postId, pageNumber, pageSize, ct);

        var responses = comments
            .Select(comment => comment.ToCommentResponse())
            .ToList();

        return ServiceResult<IReadOnlyList<CommentResponse>>.Ok(responses);
    }

    public async Task<ServiceResult<CommentResponse>> CreateCommentAsync(
        string actorUserId,
        string postId,
        CommentCreateRequest request,
        CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post is null)
        {
            return ServiceResult<CommentResponse>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        var userExists = await _userRepository.ExistsByIdAsync(actorUserId, ct);
        if (!userExists)
        {
            return ServiceResult<CommentResponse>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var now = DateTime.UtcNow;

        var comment = new Comment
        {
            PostId = postId,
            UserId = actorUserId,
            Content = request.Content,
            CreatedAt = now,
            UpdatedAt = now
        };

        await _commentRepository.AddAsync(comment, ct);

        return ServiceResult<CommentResponse>.Ok(comment.ToCommentResponse());
    }

    public async Task<ServiceResult<string>> DeleteCommentAsync(
        string postId,
        string commentId,
        CancellationToken ct = default)
    {
        var deleted = await _commentRepository.DeleteAsync(postId, commentId, ct);

        if (!deleted)
        {
            return ServiceResult<string>.Fail(ServiceErrorType.NotFound, "Comment not found.");
        }

        return ServiceResult<string>.Ok("Comment deleted.");
    }

    public async Task<ServiceResult<LikePostResult>> LikePostAsync(
        string actorUserId,
        string postId,
        LikeCreateRequest request,
        CancellationToken ct = default)
    {
        var userExists = await _userRepository.ExistsByIdAsync(actorUserId, ct);
        if (!userExists)
        {
            return ServiceResult<LikePostResult>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var postExists = await _postRepository.ExistsByIdAsync(postId, ct);

            if (!postExists)
            {
                await transaction.RollbackAsync(ct);
                return ServiceResult<LikePostResult>.Fail(ServiceErrorType.NotFound, "Post not found.");
            }

            var existingLike = await _likeRepository.GetByPostAndUserAsync(postId, actorUserId, ct);

            if (existingLike is not null)
            {
                await transaction.RollbackAsync(ct);

                return ServiceResult<LikePostResult>.Ok(new LikePostResult
                {
                    Like = existingLike.ToLikeResponse(),
                    IsCreated = false
                });
            }

            var now = DateTime.UtcNow;
            var like = new Like
            {
                PostId = postId,
                UserId = actorUserId,
                CreatedAt = now
            };

            await _likeRepository.AddAsync(like, ct);

            var updatedRows = await _postRepository.IncrementLikeCountAsync(postId, 1, ct);

            if (!updatedRows)
            {
                await transaction.RollbackAsync(ct);
                return ServiceResult<LikePostResult>.Fail(ServiceErrorType.NotFound, "Post not found.");
            }

            await transaction.CommitAsync(ct);

            return ServiceResult<LikePostResult>.Ok(new LikePostResult
            {
                Like = like.ToLikeResponse(),
                IsCreated = true
            });
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<ServiceResult<PostReportResponse>> ReportPostAsync(
        string actorUserId,
        string postId,
        PostReportCreateRequest request,
        CancellationToken ct = default)
    {
        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post is null)
        {
            return ServiceResult<PostReportResponse>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        var userExists = await _userRepository.ExistsByIdAsync(actorUserId, ct);
        if (!userExists)
        {
            return ServiceResult<PostReportResponse>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var existingReport = await _postReportRepository.ExistsByPostAndReporterAsync(
            postId,
            actorUserId,
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
            ReporterUserId = actorUserId,
            Reason = request.Reason,
            Description = request.Description,
            Status = false,
            CreatedAt = DateTime.UtcNow
        };

        await _postReportRepository.AddAsync(postReport, ct);

        return ServiceResult<PostReportResponse>.Ok(postReport.ToPostReportResponse());
    }
}
