using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Dtos;
using SocialNetwork.Model;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/users")]
public class UsersController : ApiControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public UsersController(ApplicationDbContext dbContext, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    /// <summary>Get all users.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _dbContext.Users.AsNoTracking()
            .OrderBy(user => user.UserName)
            .Select(user => new UserResponse
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ProfilePicture = user.ProfilePicture,
                Bio = user.Bio,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive
            })
            .ToListAsync();

        return OkResponse(users);
    }

    /// <summary>Get a user by id.</summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var user = await _dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Id == userId);

        if (user == null)
        {
            return NotFoundResponse("User not found.");
        }

        var response = new UserResponse
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            ProfilePicture = user.ProfilePicture,
            Bio = user.Bio,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive
        };

        return OkResponse(response);
    }

    /// <summary>Update a user profile.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// PUT /api/users/{userId}
    /// {
    ///   "profilePicture": "https://example.com/avatar.png",
    ///   "bio": "Updated bio"
    /// }
    /// </code>
    /// </remarks>
    [HttpPut("{userId}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserUpdateRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFoundResponse("User not found.");
        }

        if (request.ProfilePicture != null)
        {
            user.ProfilePicture = request.ProfilePicture;
        }

        if (request.Bio != null)
        {
            user.Bio = request.Bio;
        }

        if (request.IsActive.HasValue)
        {
            user.IsActive = request.IsActive.Value;
        }

        user.UpdatedAt = DateTime.UtcNow;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description);
            return BadRequestResponse(errors);
        }

        var response = new UserResponse
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            ProfilePicture = user.ProfilePicture,
            Bio = user.Bio,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive
        };

        return OkResponse(response);
    }

    /// <summary>Get posts authored by a user.</summary>
    [HttpGet("{userId}/posts")]
    [ProducesResponseType(typeof(ApiResponse<List<PostResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserPosts(string userId)
    {
        var userExists = await _dbContext.Users.AnyAsync(entity => entity.Id == userId);
        if (!userExists)
        {
            return NotFoundResponse("User not found.");
        }

        var posts = await _dbContext.Posts.AsNoTracking()
            .Where(post => post.UserId == userId)
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
}
