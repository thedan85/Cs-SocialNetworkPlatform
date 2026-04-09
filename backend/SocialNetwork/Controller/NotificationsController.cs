using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Dtos;
using SocialNetwork.Model;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ApiControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public NotificationsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>Get notifications for a user.</summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<NotificationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotifications(string userId)
    {
        var notifications = await _dbContext.Notifications.AsNoTracking()
            .Where(notification => notification.RecipientUserId == userId)
            .OrderByDescending(notification => notification.CreatedAt)
            .Select(notification => new NotificationResponse
            {
                NotificationId = notification.NotificationId,
                RecipientUserId = notification.RecipientUserId,
                SenderUserId = notification.SenderUserId,
                Type = notification.Type,
                Content = notification.Content,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead
            })
            .ToListAsync();

        return OkResponse(notifications);
    }

    /// <summary>Get unread notifications for a user.</summary>
    [HttpGet("user/{userId}/unread")]
    [ProducesResponseType(typeof(ApiResponse<List<NotificationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadNotifications(string userId)
    {
        var notifications = await _dbContext.Notifications.AsNoTracking()
            .Where(notification => notification.RecipientUserId == userId && !notification.IsRead)
            .OrderByDescending(notification => notification.CreatedAt)
            .Select(notification => new NotificationResponse
            {
                NotificationId = notification.NotificationId,
                RecipientUserId = notification.RecipientUserId,
                SenderUserId = notification.SenderUserId,
                Type = notification.Type,
                Content = notification.Content,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead
            })
            .ToListAsync();

        return OkResponse(notifications);
    }

    /// <summary>Create a notification.</summary>
    /// <remarks>
    /// Example request:
    /// <code>
    /// POST /api/notifications
    /// {
    ///   "recipientUserId": "user-456",
    ///   "senderUserId": "user-123",
    ///   "type": "FriendRequest",
    ///   "content": "User 123 sent you a friend request"
    /// }
    /// </code>
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<NotificationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateNotification([FromBody] NotificationCreateRequest request)
    {
        var recipientExists = await _dbContext.Users.AnyAsync(user => user.Id == request.RecipientUserId);
        var senderExists = await _dbContext.Users.AnyAsync(user => user.Id == request.SenderUserId);
        if (!recipientExists || !senderExists)
        {
            return NotFoundResponse("Recipient or sender was not found.");
        }

        var notification = new Notification
        {
            RecipientUserId = request.RecipientUserId,
            SenderUserId = request.SenderUserId,
            Type = request.Type,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _dbContext.Notifications.Add(notification);
        await _dbContext.SaveChangesAsync();

        var response = new NotificationResponse
        {
            NotificationId = notification.NotificationId,
            RecipientUserId = notification.RecipientUserId,
            SenderUserId = notification.SenderUserId,
            Type = notification.Type,
            Content = notification.Content,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead
        };

        return CreatedResponse(response);
    }

    /// <summary>Mark a notification as read.</summary>
    [HttpPut("{notificationId}/read")]
    [ProducesResponseType(typeof(ApiResponse<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(string notificationId)
    {
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(entity => entity.NotificationId == notificationId);

        if (notification == null)
        {
            return NotFoundResponse("Notification not found.");
        }

        notification.IsRead = true;
        await _dbContext.SaveChangesAsync();

        var response = new NotificationResponse
        {
            NotificationId = notification.NotificationId,
            RecipientUserId = notification.RecipientUserId,
            SenderUserId = notification.SenderUserId,
            Type = notification.Type,
            Content = notification.Content,
            CreatedAt = notification.CreatedAt,
            IsRead = notification.IsRead
        };

        return OkResponse(response);
    }

    /// <summary>Delete a notification.</summary>
    [HttpDelete("{notificationId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteNotification(string notificationId)
    {
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(entity => entity.NotificationId == notificationId);

        if (notification == null)
        {
            return NotFoundResponse("Notification not found.");
        }

        _dbContext.Notifications.Remove(notification);
        await _dbContext.SaveChangesAsync();

        return OkResponse(new { message = "Notification deleted." });
    }
}
