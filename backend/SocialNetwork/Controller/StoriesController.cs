using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Dtos;
using SocialNetwork.Model;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/stories")]
[Authorize]
public class StoriesController : ApiControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public StoriesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>Get active stories.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<StoryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStories()
    {
        var stories = await _dbContext.Stories.AsNoTracking()
            .Where(story => story.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(story => story.CreatedAt)
            .Select(story => new StoryResponse
            {
                StoryId = story.StoryId,
                UserId = story.UserId,
                Content = story.Content,
                ImageUrl = story.ImageUrl,
                CreatedAt = story.CreatedAt,
                ExpiresAt = story.ExpiresAt
            })
            .ToListAsync();

        return OkResponse(stories);
    }

    /// <summary>Get a story by id.</summary>
    [HttpGet("{storyId}")]
    [ProducesResponseType(typeof(ApiResponse<StoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStoryById(string storyId)
    {
        var story = await _dbContext.Stories.AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.StoryId == storyId);

        if (story == null)
        {
            return NotFoundResponse("Story not found.");
        }

        var response = new StoryResponse
        {
            StoryId = story.StoryId,
            UserId = story.UserId,
            Content = story.Content,
            ImageUrl = story.ImageUrl,
            CreatedAt = story.CreatedAt,
            ExpiresAt = story.ExpiresAt
        };

        return OkResponse(response);
    }

    /// <summary>Get stories for a user.</summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<StoryResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStoriesForUser(string userId)
    {
        var stories = await _dbContext.Stories.AsNoTracking()
            .Where(story => story.UserId == userId && story.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(story => story.CreatedAt)
            .Select(story => new StoryResponse
            {
                StoryId = story.StoryId,
                UserId = story.UserId,
                Content = story.Content,
                ImageUrl = story.ImageUrl,
                CreatedAt = story.CreatedAt,
                ExpiresAt = story.ExpiresAt
            })
            .ToListAsync();

        return OkResponse(stories);
    }

    /// <summary>Create a new story.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// POST /api/stories
    /// {
    ///   "userId": "user-123",
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
        var userExists = await _dbContext.Users.AnyAsync(user => user.Id == request.UserId);
        if (!userExists)
        {
            return NotFoundResponse("User not found.");
        }

        var expiresAt = request.ExpiresAt ?? DateTime.UtcNow.AddHours(24);

        var story = new Story
        {
            UserId = request.UserId,
            Content = request.Content,
            ImageUrl = request.ImageUrl,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };

        _dbContext.Stories.Add(story);
        await _dbContext.SaveChangesAsync();

        var response = new StoryResponse
        {
            StoryId = story.StoryId,
            UserId = story.UserId,
            Content = story.Content,
            ImageUrl = story.ImageUrl,
            CreatedAt = story.CreatedAt,
            ExpiresAt = story.ExpiresAt
        };

        return CreatedResponse(response);
    }

    /// <summary>Delete a story.</summary>
    [HttpDelete("{storyId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteStory(string storyId)
    {
        var story = await _dbContext.Stories.FirstOrDefaultAsync(entity => entity.StoryId == storyId);
        if (story == null)
        {
            return NotFoundResponse("Story not found.");
        }

        _dbContext.Stories.Remove(story);
        await _dbContext.SaveChangesAsync();

        return OkResponse(new { message = "Story deleted." });
    }
}
