using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Dtos;
using SocialNetwork.Service;

namespace SocialNetwork.Controller;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationsController : ApiControllerBase
{
    private readonly INotificationsService _notificationsService;

    public NotificationsController(INotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    /// <summary>Get notifications for a user.</summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<List<NotificationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetNotifications(
        string userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _notificationsService.GetNotificationsAsync(userId, pageNumber, pageSize, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Get unread notifications for a user.</summary>
    [HttpGet("user/{userId}/unread")]
    [ProducesResponseType(typeof(ApiResponse<List<NotificationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadNotifications(
        string userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var result = await _notificationsService.GetUnreadNotificationsAsync(userId, pageNumber, pageSize, HttpContext.RequestAborted);
        return FromServiceResult(result);
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
        var result = await _notificationsService.CreateNotificationAsync(request, HttpContext.RequestAborted);
        return FromServiceResult(result, created: true);
    }

    /// <summary>Mark a notification as read.</summary>
    [HttpPut("{notificationId}/read")]
    [ProducesResponseType(typeof(ApiResponse<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(string notificationId)
    {
        var result = await _notificationsService.MarkAsReadAsync(notificationId, HttpContext.RequestAborted);
        return FromServiceResult(result);
    }

    /// <summary>Delete a notification.</summary>
    [HttpDelete("{notificationId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteNotification(string notificationId)
    {
        var result = await _notificationsService.DeleteNotificationAsync(notificationId, HttpContext.RequestAborted);
        if (!result.Success)
        {
            return FromServiceResult(result);
        }

        return OkResponse(new { message = result.Data });
    }
}
