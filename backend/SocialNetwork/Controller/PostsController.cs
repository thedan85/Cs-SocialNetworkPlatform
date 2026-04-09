using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Dtos;
using SocialNetwork.Model;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/posts")]
[Authorize]
public class PostsController : ApiControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public PostsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>Get all posts.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<PostResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPosts()
    {
        var posts = await _dbContext.Posts.AsNoTracking()
            .OrderByDescending(post => post.CreatedAt)
            .Select(post => new PostResponse
            {
                PostId = post.PostId,
                UserId = post.UserId,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                LikeCount = post.LikeCount,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            })
            .ToListAsync();

        return OkResponse(posts);
    }

    /// <summary>Get a single post by id.</summary>
    [HttpGet("{postId}")]
    [ProducesResponseType(typeof(ApiResponse<PostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostById(string postId)
    {
        var post = await _dbContext.Posts.AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.PostId == postId);

        if (post == null)
        {
            return NotFoundResponse("Post not found.");
        }

        var response = new PostResponse
        {
            PostId = post.PostId,
            UserId = post.UserId,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            LikeCount = post.LikeCount,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };

        return OkResponse(response);
    }

    /// <summary>Create a new post.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// POST /api/posts
    /// {
    ///   "userId": "user-123",
    ///   "content": "Hello world",
    ///   "imageUrl": "https://example.com/image.png"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PostResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreatePost([FromBody] PostCreateRequest request)
    {
        var userExists = await _dbContext.Users.AnyAsync(user => user.Id == request.UserId);
        if (!userExists)
        {
            return NotFoundResponse("User not found.");
        }

        var post = new Post
        {
            UserId = request.UserId,
            Content = request.Content,
            ImageUrl = request.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Posts.Add(post);
        await _dbContext.SaveChangesAsync();

        var response = new PostResponse
        {
            PostId = post.PostId,
            UserId = post.UserId,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            LikeCount = post.LikeCount,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };

        return CreatedResponse(response);
    }

    /// <summary>Update an existing post.</summary>
    [HttpPut("{postId}")]
    [ProducesResponseType(typeof(ApiResponse<PostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePost(string postId, [FromBody] PostUpdateRequest request)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(entity => entity.PostId == postId);
        if (post == null)
        {
            return NotFoundResponse("Post not found.");
        }

        post.Content = request.Content;
        post.ImageUrl = request.ImageUrl;
        post.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        var response = new PostResponse
        {
            PostId = post.PostId,
            UserId = post.UserId,
            Content = post.Content,
            ImageUrl = post.ImageUrl,
            LikeCount = post.LikeCount,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        };

        return OkResponse(response);
    }

    /// <summary>Delete a post.</summary>
    [HttpDelete("{postId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost(string postId)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(entity => entity.PostId == postId);
        if (post == null)
        {
            return NotFoundResponse("Post not found.");
        }

        _dbContext.Posts.Remove(post);
        await _dbContext.SaveChangesAsync();

        return OkResponse(new { message = "Post deleted." });
    }

    /// <summary>Get comments for a post.</summary>
    [HttpGet("{postId}/comments")]
    [ProducesResponseType(typeof(ApiResponse<List<CommentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostComments(string postId)
    {
        var postExists = await _dbContext.Posts.AnyAsync(entity => entity.PostId == postId);
        if (!postExists)
        {
            return NotFoundResponse("Post not found.");
        }

        var comments = await _dbContext.Comments.AsNoTracking()
            .Where(comment => comment.PostId == postId)
            .OrderByDescending(comment => comment.CreatedAt)
            .Select(comment => new CommentResponse
            {
                CommentId = comment.CommentId,
                PostId = comment.PostId,
                UserId = comment.UserId,
                Content = comment.Content,
                LikeCount = comment.LikeCount,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            })
            .ToListAsync();

        return OkResponse(comments);
    }

    /// <summary>Add a comment to a post.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// POST /api/posts/{postId}/comments
    /// {
    ///   "userId": "user-123",
    ///   "content": "Nice post!"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost("{postId}/comments")]
    [ProducesResponseType(typeof(ApiResponse<CommentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateComment(string postId, [FromBody] CommentCreateRequest request)
    {
        var postExists = await _dbContext.Posts.AnyAsync(entity => entity.PostId == postId);
        if (!postExists)
        {
            return NotFoundResponse("Post not found.");
        }

        var userExists = await _dbContext.Users.AnyAsync(user => user.Id == request.UserId);
        if (!userExists)
        {
            return NotFoundResponse("User not found.");
        }

        var comment = new Comment
        {
            PostId = postId,
            UserId = request.UserId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Comments.Add(comment);
        await _dbContext.SaveChangesAsync();

        var response = new CommentResponse
        {
            CommentId = comment.CommentId,
            PostId = comment.PostId,
            UserId = comment.UserId,
            Content = comment.Content,
            LikeCount = comment.LikeCount,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };

        return CreatedResponse(response);
    }

    /// <summary>Delete a comment on a post.</summary>
    [HttpDelete("{postId}/comments/{commentId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(string postId, string commentId)
    {
        var comment = await _dbContext.Comments
            .FirstOrDefaultAsync(entity => entity.PostId == postId && entity.CommentId == commentId);

        if (comment == null)
        {
            return NotFoundResponse("Comment not found.");
        }

        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync();

        return OkResponse(new { message = "Comment deleted." });
    }

    /// <summary>Like a post.</summary>
    [HttpPost("{postId}/likes")]
    [ProducesResponseType(typeof(ApiResponse<LikeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LikeResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LikePost(string postId, [FromBody] LikeCreateRequest request)
    {
        var post = await _dbContext.Posts.FirstOrDefaultAsync(entity => entity.PostId == postId);
        if (post == null)
        {
            return NotFoundResponse("Post not found.");
        }

        var userExists = await _dbContext.Users.AnyAsync(user => user.Id == request.UserId);
        if (!userExists)
        {
            return NotFoundResponse("User not found.");
        }

        var existingLike = await _dbContext.Likes
            .FirstOrDefaultAsync(entity => entity.PostId == postId && entity.UserId == request.UserId);

        if (existingLike != null)
        {
            var existingResponse = new LikeResponse
            {
                LikeId = existingLike.LikeId,
                UserId = existingLike.UserId,
                PostId = existingLike.PostId,
                CreatedAt = existingLike.CreatedAt
            };

            return OkResponse(existingResponse);
        }

        var like = new Like
        {
            PostId = postId,
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Likes.Add(like);
        post.LikeCount += 1;
        await _dbContext.SaveChangesAsync();

        var response = new LikeResponse
        {
            LikeId = like.LikeId,
            UserId = like.UserId,
            PostId = like.PostId,
            CreatedAt = like.CreatedAt
        };

        return CreatedResponse(response);
    }

    /// <summary>Report a post.</summary>
    [HttpPost("{postId}/report")]
    [ProducesResponseType(typeof(ApiResponse<PostReportResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReportPost(string postId, [FromBody] PostReportCreateRequest request)
    {
        var postExists = await _dbContext.Posts.AnyAsync(entity => entity.PostId == postId);
        if (!postExists)
        {
            return NotFoundResponse("Post not found.");
        }

        var userExists = await _dbContext.Users.AnyAsync(user => user.Id == request.ReporterUserId);
        if (!userExists)
        {
            return NotFoundResponse("User not found.");
        }

        var report = new PostReport
        {
            PostId = postId,
            ReporterUserId = request.ReporterUserId,
            Reason = request.Reason,
            Description = request.Description,
            Status = false,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.PostReports.Add(report);
        await _dbContext.SaveChangesAsync();

        var response = new PostReportResponse
        {
            PostReportId = report.PostReportId,
            PostId = report.PostId,
            ReporterUserId = report.ReporterUserId,
            Reason = report.Reason,
            Description = report.Description,
            Status = report.Status,
            CreatedAt = report.CreatedAt
        };

        return CreatedResponse(response);
    }
}
