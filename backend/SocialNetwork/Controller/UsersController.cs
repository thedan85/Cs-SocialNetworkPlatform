using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Dtos;
using SocialNetwork.Service;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ApiControllerBase
{
    private readonly IUsersService _usersService;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
    }

    /// <summary>Get all users.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<List<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _usersService.GetUsersAsync(HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Search users by name or username.</summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<List<UserResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string query,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _usersService.SearchUsersAsync(query, pageNumber, pageSize, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Get a user by id.</summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(string userId)
    {
        var currentUserId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return UnauthorizedResponse("User context is missing.");
        }

        var result = await _usersService.GetUserByIdAsync(
            currentUserId,
            userId,
            User.IsInRole("Admin"),
            HttpContext.RequestAborted);
        return FromServiceResult(result);
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
        if (!IsCurrentUserOrAdmin(userId))
        {
            return UnauthorizedResponse("You are not allowed to update this user.");
        }

        var result = await _usersService.UpdateUserAsync(userId, request, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Get posts authored by a user.</summary>
    [HttpGet("{userId}/posts")]
    [ProducesResponseType(typeof(ApiResponse<List<PostResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserPosts(string userId)
    {
        var currentUserId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return UnauthorizedResponse("User context is missing.");
        }

        var result = await _usersService.GetUserPostsAsync(
            currentUserId,
            userId,
            User.IsInRole("Admin"),
            HttpContext.RequestAborted);
        return FromServiceResult(result);
    }
}
