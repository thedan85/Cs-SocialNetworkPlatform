using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Dtos;
using SocialNetwork.Model;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly UserManager<User> _userManager;

    public AuthController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>Register a new user.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// POST /api/auth/register
    /// {
    ///   "userName": "jane",
    ///   "email": "jane@example.com",
    ///   "password": "Pass1234!",
    ///   "profilePicture": "https://example.com/avatar.png",
    ///   "bio": "Hello there"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthUserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
        if (existingByEmail != null)
        {
            return BadRequestResponse("Email is already in use.");
        }

        var existingByUserName = await _userManager.FindByNameAsync(request.UserName);
        if (existingByUserName != null)
        {
            return BadRequestResponse("Username is already in use.");
        }

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            ProfilePicture = request.ProfilePicture,
            Bio = request.Bio,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(error => error.Description);
            return BadRequestResponse(errors);
        }

        var response = new AuthUserResponse
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            ProfilePicture = user.ProfilePicture,
            Bio = user.Bio
        };

        return CreatedResponse(response);
    }

    /// <summary>Log in with username or email.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// POST /api/auth/login
    /// {
    ///   "userNameOrEmail": "jane@example.com",
    ///   "password": "Pass1234!"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.UserNameOrEmail)
            ?? await _userManager.FindByNameAsync(request.UserNameOrEmail);

        if (user == null)
        {
            return UnauthorizedResponse("Invalid credentials.");
        }

        var isValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isValid)
        {
            return UnauthorizedResponse("Invalid credentials.");
        }

        var response = new AuthUserResponse
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            ProfilePicture = user.ProfilePicture,
            Bio = user.Bio
        };

        return OkResponse(response);
     }
 }
