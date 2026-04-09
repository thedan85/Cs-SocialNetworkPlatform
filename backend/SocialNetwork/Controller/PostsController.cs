using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Dtos;
using SocialNetwork.Service;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/posts")]
[Authorize]
public class PostsController : ApiControllerBase
{
    private readonly IPostsService _postsService;

    public PostsController(IPostsService postsService)
    {
        _postsService = postsService;
    }

    /// <summary>Get all posts.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<PostResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPosts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _postsService.GetPostsAsync(pageNumber, pageSize, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Get a single post by id.</summary>
    [HttpGet("{postId}")]
    [ProducesResponseType(typeof(ApiResponse<PostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostById(string postId)
    {
        var result = await _postsService.GetPostByIdAsync(postId, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Create a new post.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// POST /api/posts
    /// {
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
        var currentUserId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return UnauthorizedResponse("User identity is missing.");
        }

        var result = await _postsService.CreatePostAsync(currentUserId, request, HttpContext.RequestAborted);
        return FromServiceResult(result, created: true);
    }

    /// <summary>Update an existing post.</summary>
    [HttpPut("{postId}")]
    [ProducesResponseType(typeof(ApiResponse<PostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePost(string postId, [FromBody] PostUpdateRequest request)
    {
        var result = await _postsService.UpdatePostAsync(postId, request, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Delete a post.</summary>
    [HttpDelete("{postId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePost(string postId)
    {
        var result = await _postsService.DeletePostAsync(postId, HttpContext.RequestAborted);
        if (!result.Success)
        {
            return FromServiceResult(result);
        }

        return OkResponse(new { message = result.Data });
    }

    /// <summary>Get comments for a post.</summary>
    [HttpGet("{postId}/comments")]
    [ProducesResponseType(typeof(ApiResponse<List<CommentResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostComments(
        string postId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _postsService.GetPostCommentsAsync(postId, pageNumber, pageSize, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Add a comment to a post.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// POST /api/posts/{postId}/comments
    /// {
    ///   "content": "Nice post!"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost("{postId}/comments")]
    [ProducesResponseType(typeof(ApiResponse<CommentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateComment(string postId, [FromBody] CommentCreateRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return UnauthorizedResponse("User identity is missing.");
        }

        var result = await _postsService.CreateCommentAsync(currentUserId, postId, request, HttpContext.RequestAborted);
        return FromServiceResult(result, created: true);
    }

    /// <summary>Delete a comment on a post.</summary>
    [HttpDelete("{postId}/comments/{commentId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteComment(string postId, string commentId)
    {
        var result = await _postsService.DeleteCommentAsync(postId, commentId, HttpContext.RequestAborted);
        if (!result.Success)
        {
            return FromServiceResult(result);
        }

        return OkResponse(new { message = result.Data });
    }

    /// <summary>Like a post.</summary>
    [HttpPost("{postId}/likes")]
    [ProducesResponseType(typeof(ApiResponse<LikeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<LikeResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> LikePost(string postId, [FromBody] LikeCreateRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return UnauthorizedResponse("User identity is missing.");
        }

        var result = await _postsService.LikePostAsync(currentUserId, postId, request, HttpContext.RequestAborted);
        if (!result.Success)
        {
            return FromServiceResult(result);
        }

        if (result.Data is null)
        {
            return BadRequestResponse("Unable to process like.");
        }

        if (result.Data.IsCreated)
        {
            return CreatedResponse(result.Data.Like);
        }

        return OkResponse(result.Data.Like);
    }

    /// <summary>Report a post.</summary>
    [HttpPost("{postId}/report")]
    [ProducesResponseType(typeof(ApiResponse<PostReportResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ReportPost(string postId, [FromBody] PostReportCreateRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return UnauthorizedResponse("User identity is missing.");
        }

        var result = await _postsService.ReportPostAsync(currentUserId, postId, request, HttpContext.RequestAborted);
        return FromServiceResult(result, created: true);
    }
}
