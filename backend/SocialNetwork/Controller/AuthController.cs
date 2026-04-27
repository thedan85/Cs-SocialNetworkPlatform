using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Dtos;
using SocialNetwork.Model;
using SocialNetwork.Service;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/auth")]
public class AuthController : ApiControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private const string SessionTokenProvider = "SessionToken";
    private const string SessionTokenName = "SessionToken";
    private const string SessionTokenCookieName = "session_token";

    public AuthController(UserManager<User> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
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
    ///   "bio": "Hello there"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost("register")]
    [AllowAnonymous]
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
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            Email = request.Email,
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

        var roleResult = await _userManager.AddToRoleAsync(user, "User");
        if (!roleResult.Succeeded)
        {
            var errors = roleResult.Errors.Select(error => error.Description);
            return BadRequestResponse(errors);
        }

        var response = new AuthUserResponse
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
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
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
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

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.CreateToken(user, roles);
        var sessionToken = _jwtTokenService.CreateSessionToken(user);
        var sessionResult = await _userManager.SetAuthenticationTokenAsync(
            user,
            SessionTokenProvider,
            SessionTokenName,
            sessionToken.Token);
        if (!sessionResult.Succeeded)
        {
            var errors = sessionResult.Errors.Select(error => error.Description);
            return BadRequestResponse(errors);
        }

        SetSessionTokenCookie(sessionToken.Token, sessionToken.ExpiresAt);

        var response = new AuthTokenResponse
        {
            User = MapUser(user),
            Token = token
        };

        return OkResponse(response);
     }

    /// <summary>Refresh access token using session token cookie.</summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue(SessionTokenCookieName, out var sessionToken)
            || string.IsNullOrWhiteSpace(sessionToken))
        {
            return UnauthorizedResponse("Session token is missing.");
        }

        var userId = _jwtTokenService.GetUserIdFromSessionToken(sessionToken);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return UnauthorizedResponse("Invalid session token.");
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return UnauthorizedResponse("Invalid session token.");
        }

        var storedToken = await _userManager.GetAuthenticationTokenAsync(
            user,
            SessionTokenProvider,
            SessionTokenName);
        if (string.IsNullOrWhiteSpace(storedToken)
            || !string.Equals(storedToken, sessionToken, StringComparison.Ordinal))
        {
            return UnauthorizedResponse("Invalid session token.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.CreateToken(user, roles);

        var response = new AuthTokenResponse
        {
            User = MapUser(user),
            Token = token
        };

        return OkResponse(response);
    }

    private static AuthUserResponse MapUser(User user)
    {
        return new AuthUserResponse
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Bio = user.Bio
        };
    }

    private void SetSessionTokenCookie(string token, DateTime expiresAt)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = expiresAt,
            Path = "/"
        };

        Response.Cookies.Append(SessionTokenCookieName, token, options);
    }
 }
