using SocialNetwork.Dtos;
using SocialNetwork.Extensions;
using SocialNetwork.Helpers;
using SocialNetwork.Model;
using SocialNetwork.Repository;

namespace SocialNetwork.Service;

public class PostsService : IPostsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ILikeRepository _likeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHashtagRepository _hashtagRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IPostReportRepository _postReportRepository;

    public PostsService(
        IUnitOfWork unitOfWork,
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        ILikeRepository likeRepository,
        IUserRepository userRepository,
        IHashtagRepository hashtagRepository,
        IFriendshipRepository friendshipRepository,
        IPostReportRepository postReportRepository)
    {
        _unitOfWork = unitOfWork;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _likeRepository = likeRepository;
        _userRepository = userRepository;
        _hashtagRepository = hashtagRepository;
        _friendshipRepository = friendshipRepository;
        _postReportRepository = postReportRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<PostResponse>>> GetPostsAsync(
        string actorUserId,
        bool isAdmin,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<IReadOnlyList<PostResponse>>.Fail(
                ServiceErrorType.Unauthorized,
                "User context is missing.");
        }

        var posts = await _postRepository.GetVisiblePagedAsync(actorUserId, isAdmin, pageNumber, pageSize, ct);

        var responses = posts
            .Select(post => post.ToPostResponse())
            .ToList();

        return ServiceResult<IReadOnlyList<PostResponse>>.Ok(responses);
    }

    public async Task<ServiceResult<PostResponse>> GetPostByIdAsync(
        string actorUserId,
        string postId,
        bool isAdmin,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<PostResponse>.Fail(ServiceErrorType.Unauthorized, "User context is missing.");
        }

        var postResult = await GetVisiblePostAsync(actorUserId, postId, isAdmin, ct);
        if (!postResult.Success || postResult.Data is null)
        {
            return ServiceResult<PostResponse>.Fail(
                postResult.ErrorType ?? ServiceErrorType.Validation,
                postResult.Errors);
        }

        return ServiceResult<PostResponse>.Ok(postResult.Data.ToPostResponse());
    }

    public async Task<ServiceResult<PostResponse>> CreatePostAsync(
        string actorUserId,
        PostCreateRequest request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<PostResponse>.Fail(ServiceErrorType.Unauthorized, "User context is missing.");
        }

        var user = await _userRepository.GetByIdAsync(actorUserId, ct);
        if (user is null)
        {
            return ServiceResult<PostResponse>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var post = new Post
        {
            UserId = actorUserId,
            Content = request.Content,
            ImageUrl = request.ImageUrl,
            Privacy = NormalizePrivacy(request.Privacy) ?? PostPrivacy.Public,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (!IsPrivacyAllowed(post.Privacy))
        {
            return ServiceResult<PostResponse>.Fail(ServiceErrorType.Validation, "Invalid privacy setting.");
        }

        var tags = HashtagHelper.ExtractTags(request.Content);

        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            await _postRepository.AddAsync(post, ct);

            if (tags.Count > 0)
            {
                var hashtags = await _hashtagRepository.AddOrUpdateAsync(tags, ct);
                await _hashtagRepository.AddPostHashtagsAsync(
                    post.PostId,
                    hashtags.Select(tag => tag.HashtagId),
                    ct);
            }

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }

        var response = new PostResponse
        {
            PostId = post.PostId,
            UserId = post.UserId,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            Privacy = post.Privacy,
            LikeCount = post.LikeCount,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };

        return ServiceResult<PostResponse>.Ok(response);
    }

    public async Task<ServiceResult<PostResponse>> UpdatePostAsync(
        string actorUserId,
        string postId,
        PostUpdateRequest request,
        bool isAdmin,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<PostResponse>.Fail(ServiceErrorType.Unauthorized, "User context is missing.");
        }

        var existingPost = await _postRepository.GetByIdAsync(postId, ct);
        if (existingPost is null)
        {
            return ServiceResult<PostResponse>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        if (!isAdmin && !string.Equals(existingPost.UserId, actorUserId, StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<PostResponse>.Fail(ServiceErrorType.Unauthorized, "You are not allowed to update this post.");
        }

        var now = DateTime.UtcNow;

        existingPost.Content = request.Content;
        existingPost.ImageUrl = request.ImageUrl;
        if (!string.IsNullOrWhiteSpace(request.Privacy))
        {
            var privacy = NormalizePrivacy(request.Privacy);
            if (privacy is null || !IsPrivacyAllowed(privacy))
            {
                return ServiceResult<PostResponse>.Fail(ServiceErrorType.Validation, "Invalid privacy setting.");
            }

            existingPost.Privacy = privacy;
        }

        existingPost.UpdatedAt = now;

        await _postRepository.UpdateAsync(existingPost, ct);
        return ServiceResult<PostResponse>.Ok(existingPost.ToPostResponse());
    }

    public async Task<ServiceResult<string>> DeletePostAsync(
        string actorUserId,
        string postId,
        bool isAdmin,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<string>.Fail(ServiceErrorType.Unauthorized, "User context is missing.");
        }

        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post is null)
        {
            return ServiceResult<string>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        if (!isAdmin && !string.Equals(post.UserId, actorUserId, StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<string>.Fail(ServiceErrorType.Unauthorized, "You are not allowed to delete this post.");
        }

        await _postRepository.DeleteAsync(postId, ct);
        return ServiceResult<string>.Ok("Post deleted.");
    }

    public async Task<ServiceResult<IReadOnlyList<CommentResponse>>> GetPostCommentsAsync(
        string actorUserId,
        string postId,
        bool isAdmin,
        int pageNumber = 1,
        int pageSize = 20,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<IReadOnlyList<CommentResponse>>.Fail(
                ServiceErrorType.Unauthorized,
                "User context is missing.");
        }

        var postResult = await GetVisiblePostAsync(actorUserId, postId, isAdmin, ct);
        if (!postResult.Success)
        {
            return ServiceResult<IReadOnlyList<CommentResponse>>.Fail(
                postResult.ErrorType ?? ServiceErrorType.Validation,
                postResult.Errors);
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
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<CommentResponse>.Fail(ServiceErrorType.Unauthorized, "User context is missing.");
        }

        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post is null)
        {
            return ServiceResult<CommentResponse>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        var canView = await IsPostVisibleAsync(post, actorUserId, false, ct);
        if (!canView)
        {
            return ServiceResult<CommentResponse>.Fail(
                ServiceErrorType.Unauthorized,
                "You are not allowed to comment on this post.");
        }

        var user = await _userRepository.GetByIdAsync(actorUserId, ct);
        if (user is null)
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

        var response = new CommentResponse
        {
            CommentId = comment.CommentId,
            PostId = comment.PostId,
            UserId = comment.UserId,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Content = comment.Content,
            LikeCount = comment.LikeCount,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };

        return ServiceResult<CommentResponse>.Ok(response);
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
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<LikePostResult>.Fail(ServiceErrorType.Unauthorized, "User context is missing.");
        }

        var userExists = await _userRepository.ExistsByIdAsync(actorUserId, ct);
        if (!userExists)
        {
            return ServiceResult<LikePostResult>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        await using var transaction = await _unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var post = await _postRepository.GetByIdAsync(postId, ct);

            if (post is null)
            {
                await transaction.RollbackAsync(ct);
                return ServiceResult<LikePostResult>.Fail(ServiceErrorType.NotFound, "Post not found.");
            }

            var canView = await IsPostVisibleAsync(post, actorUserId, false, ct);
            if (!canView)
            {
                await transaction.RollbackAsync(ct);
                return ServiceResult<LikePostResult>.Fail(
                    ServiceErrorType.Unauthorized,
                    "You are not allowed to like this post.");
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

            var updated = await _postRepository.IncrementLikeCountAsync(postId, 1, ct);

            if (!updated)
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
        if (string.IsNullOrWhiteSpace(actorUserId))
        {
            return ServiceResult<PostReportResponse>.Fail(ServiceErrorType.Unauthorized, "User context is missing.");
        }

        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post is null)
        {
            return ServiceResult<PostReportResponse>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        var canView = await IsPostVisibleAsync(post, actorUserId, false, ct);
        if (!canView)
        {
            return ServiceResult<PostReportResponse>.Fail(
                ServiceErrorType.Unauthorized,
                "You are not allowed to report this post.");
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

    private async Task<ServiceResult<Post>> GetVisiblePostAsync(
        string actorUserId,
        string postId,
        bool isAdmin,
        CancellationToken ct)
    {
        var post = await _postRepository.GetByIdAsync(postId, ct);
        if (post is null)
        {
            return ServiceResult<Post>.Fail(ServiceErrorType.NotFound, "Post not found.");
        }

        var canView = await IsPostVisibleAsync(post, actorUserId, isAdmin, ct);
        if (!canView)
        {
            return ServiceResult<Post>.Fail(ServiceErrorType.Unauthorized, "You are not allowed to view this post.");
        }

        return ServiceResult<Post>.Ok(post);
    }

    private async Task<bool> IsPostVisibleAsync(
        Post post,
        string actorUserId,
        bool isAdmin,
        CancellationToken ct)
    {
        var privacy = string.IsNullOrWhiteSpace(post.Privacy)
            ? PostPrivacy.Public
            : post.Privacy;

        if (isAdmin || string.Equals(post.UserId, actorUserId, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (string.Equals(privacy, PostPrivacy.Public, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (string.Equals(privacy, PostPrivacy.Friends, StringComparison.OrdinalIgnoreCase))
        {
            return await _friendshipRepository.AreFriendsAsync(actorUserId, post.UserId, ct);
        }

        return false;
    }

    private static bool IsPrivacyAllowed(string privacy)
    {
        return PostPrivacy.Allowed.Any(value =>
            string.Equals(value, privacy, StringComparison.OrdinalIgnoreCase));
    }

    private static string? NormalizePrivacy(string? privacy)
    {
        if (string.IsNullOrWhiteSpace(privacy))
        {
            return null;
        }

        var trimmed = privacy.Trim();
        if (string.Equals(trimmed, PostPrivacy.Public, StringComparison.OrdinalIgnoreCase))
        {
            return PostPrivacy.Public;
        }

        if (string.Equals(trimmed, PostPrivacy.Friends, StringComparison.OrdinalIgnoreCase))
        {
            return PostPrivacy.Friends;
        }

        if (string.Equals(trimmed, PostPrivacy.Private, StringComparison.OrdinalIgnoreCase))
        {
            return PostPrivacy.Private;
        }

        return trimmed;
    }
}
