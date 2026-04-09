using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Dtos;
using SocialNetwork.Service;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/stories")]
[Authorize]
public class StoriesController : ApiControllerBase
{
    private readonly IStoriesService _storiesService;

    public StoriesController(IStoriesService storiesService)
    {
        _storiesService = storiesService;
    }

    /// <summary>Get active stories.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<StoryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStories([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var result = await _storiesService.GetStoriesAsync(pageNumber, pageSize, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Get a story by id.</summary>
    [HttpGet("{storyId}")]
    [ProducesResponseType(typeof(ApiResponse<StoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStoryById(string storyId)
    {
        var result = await _storiesService.GetStoryByIdAsync(storyId, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Get stories for a user.</summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<StoryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStoriesForUser(
        string userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        if (!IsCurrentUserOrAdmin(userId))
        {
            return UnauthorizedResponse("You are not allowed to access these stories.");
        }

        var result = await _storiesService.GetStoriesForUserAsync(userId, pageNumber, pageSize, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Create a new story.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// POST /api/stories
    /// {
    ///   "content": "My story",
    ///   "imageUrl": "https://example.com/story.png",
    ///   "expiresAt": "2026-04-10T12:00:00Z"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StoryResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateStory([FromBody] StoryCreateRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return UnauthorizedResponse("User identity is missing.");
        }

        var result = await _storiesService.CreateStoryAsync(currentUserId, request, HttpContext.RequestAborted);
        return FromServiceResult(result, created: true);
    }

    /// <summary>Delete a story.</summary>
    [HttpDelete("{storyId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStory(string storyId)
    {
        var result = await _storiesService.DeleteStoryAsync(storyId, HttpContext.RequestAborted);
        if (!result.Success)
        {
            return FromServiceResult(result);
        }

        return OkResponse(new { message = result.Data });
    }
}
