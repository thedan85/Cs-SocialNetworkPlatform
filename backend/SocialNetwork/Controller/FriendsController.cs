using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Dtos;
using SocialNetwork.Model;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/friends")]
[Authorize]
public class FriendsController : ApiControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public FriendsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>Send a friend request.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// POST /api/friends/requests
    /// {
    ///   "requesterUserId": "user-123",
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
        if (request.RequesterUserId == request.AddresseeUserId)
        {
            return BadRequestResponse("You cannot friend yourself.");
        }

        var requesterExists = await _dbContext.Users.AnyAsync(user => user.Id == request.RequesterUserId);
        var addresseeExists = await _dbContext.Users.AnyAsync(user => user.Id == request.AddresseeUserId);
        if (!requesterExists || !addresseeExists)
        {
            return NotFoundResponse("One or more users were not found.");
        }

        var existingRequest = await _dbContext.Friendships.AnyAsync(friendship =>
            (friendship.UserId1 == request.RequesterUserId && friendship.UserId2 == request.AddresseeUserId)
            || (friendship.UserId1 == request.AddresseeUserId && friendship.UserId2 == request.RequesterUserId));

        if (existingRequest)
        {
            return BadRequestResponse("A friend request already exists between these users.");
        }

        var friendship = new Friendship
        {
            UserId1 = request.RequesterUserId,
            UserId2 = request.AddresseeUserId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Friendships.Add(friendship);
        await _dbContext.SaveChangesAsync();

        var response = new FriendshipResponse
        {
            FriendshipId = friendship.FriendshipId,
            UserId1 = friendship.UserId1,
            UserId2 = friendship.UserId2,
            Status = friendship.Status,
            CreatedAt = friendship.CreatedAt,
            UpdatedAt = friendship.UpdatedAt
        };

        return CreatedResponse(response);
    }

    /// <summary>Accept a friend request.</summary>
    [HttpPut("requests/{friendshipId}/accept")]
    [ProducesResponseType(typeof(ApiResponse<FriendshipResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptFriendRequest(string friendshipId)
    {
        var friendship = await _dbContext.Friendships.FirstOrDefaultAsync(entity => entity.FriendshipId == friendshipId);
        if (friendship == null)
        {
            return NotFoundResponse("Friend request not found.");
        }

        if (!string.Equals(friendship.Status, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequestResponse("Only pending requests can be accepted.");
        }

        friendship.Status = "Accepted";
        friendship.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        var response = new FriendshipResponse
        {
            FriendshipId = friendship.FriendshipId,
            UserId1 = friendship.UserId1,
            UserId2 = friendship.UserId2,
            Status = friendship.Status,
            CreatedAt = friendship.CreatedAt,
            UpdatedAt = friendship.UpdatedAt
        };

        return OkResponse(response);
    }

    /// <summary>Reject a friend request.</summary>
    [HttpPut("requests/{friendshipId}/reject")]
    [ProducesResponseType(typeof(ApiResponse<FriendshipResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectFriendRequest(string friendshipId)
    {
        var friendship = await _dbContext.Friendships.FirstOrDefaultAsync(entity => entity.FriendshipId == friendshipId);
        if (friendship == null)
        {
            return NotFoundResponse("Friend request not found.");
        }

        if (!string.Equals(friendship.Status, "Pending", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequestResponse("Only pending requests can be rejected.");
        }

        friendship.Status = "Rejected";
        friendship.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        var response = new FriendshipResponse
        {
            FriendshipId = friendship.FriendshipId,
            UserId1 = friendship.UserId1,
            UserId2 = friendship.UserId2,
            Status = friendship.Status,
            CreatedAt = friendship.CreatedAt,
            UpdatedAt = friendship.UpdatedAt
        };

        return OkResponse(response);
    }

    /// <summary>Get accepted friends for a user.</summary>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<FriendshipResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFriends(string userId)
    {
        var friends = await _dbContext.Friendships.AsNoTracking()
            .Where(friendship => (friendship.UserId1 == userId || friendship.UserId2 == userId)
                && friendship.Status == "Accepted")
            .Select(friendship => new FriendshipResponse
            {
                FriendshipId = friendship.FriendshipId,
                UserId1 = friendship.UserId1,
                UserId2 = friendship.UserId2,
                Status = friendship.Status,
                CreatedAt = friendship.CreatedAt,
                UpdatedAt = friendship.UpdatedAt
            })
            .ToListAsync();

        return OkResponse(friends);
    }

    /// <summary>Get pending friend requests for a user.</summary>
    [HttpGet("requests/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<FriendshipResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingRequests(string userId)
    {
        var requests = await _dbContext.Friendships.AsNoTracking()
            .Where(friendship => friendship.UserId2 == userId && friendship.Status == "Pending")
            .Select(friendship => new FriendshipResponse
            {
                FriendshipId = friendship.FriendshipId,
                UserId1 = friendship.UserId1,
                UserId2 = friendship.UserId2,
                Status = friendship.Status,
                CreatedAt = friendship.CreatedAt,
                UpdatedAt = friendship.UpdatedAt
            })
            .ToListAsync();

        return OkResponse(requests);
    }
}
