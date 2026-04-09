using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Dtos;
using SocialNetwork.Service;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/friends")]
[Authorize]
public class FriendsController : ApiControllerBase
{
    private readonly IFriendsService _friendsService;

    public FriendsController(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }

    /// <summary>Send a friend request.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// POST /api/friends/requests
    /// {
    ///   "addresseeUserId": "user-456"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost("requests")]
    [ProducesResponseType(typeof(ApiResponse<FriendshipResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateFriendRequest([FromBody] FriendRequestCreateRequest request)
    {
        var currentUserId = GetCurrentUserId();
        if (string.IsNullOrWhiteSpace(currentUserId))
        {
            return UnauthorizedResponse("User identity is missing.");
        }

        var result = await _friendsService.CreateFriendRequestAsync(currentUserId, request, HttpContext.RequestAborted);
        return FromServiceResult(result, created: true);
    }

    /// <summary>Accept a friend request.</summary>
    [HttpPut("requests/{friendshipId}/accept")]
    [ProducesResponseType(typeof(ApiResponse<FriendshipResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptFriendRequest(string friendshipId)
    {
        var result = await _friendsService.AcceptFriendRequestAsync(friendshipId, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Reject a friend request.</summary>
    [HttpPut("requests/{friendshipId}/reject")]
    [ProducesResponseType(typeof(ApiResponse<FriendshipResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectFriendRequest(string friendshipId)
    {
        var result = await _friendsService.RejectFriendRequestAsync(friendshipId, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Get accepted friends for a user.</summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<FriendshipResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFriends(
        string userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        if (!IsCurrentUserOrAdmin(userId))
        {
            return UnauthorizedResponse("You are not allowed to access these friends.");
        }

        var result = await _friendsService.GetFriendsAsync(userId, pageNumber, pageSize, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Get pending friend requests for a user.</summary>
    [HttpGet("requests/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<FriendshipResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingRequests(
        string userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        if (!IsCurrentUserOrAdmin(userId))
        {
            return UnauthorizedResponse("You are not allowed to access these requests.");
        }

        var result = await _friendsService.GetPendingRequestsAsync(userId, pageNumber, pageSize, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }
}
