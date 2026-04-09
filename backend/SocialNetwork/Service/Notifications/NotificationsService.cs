using SocialNetwork.Dtos;
using SocialNetwork.Extensions;
using SocialNetwork.Model;
using SocialNetwork.Repository;

namespace SocialNetwork.Service;

public class NotificationsService : INotificationsService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;

    public NotificationsService(
        INotificationRepository notificationRepository,
        IUserRepository userRepository)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
    }

    public async Task<ServiceResult<IReadOnlyList<NotificationResponse>>> GetNotificationsAsync(
        string userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        var userExists = await _userRepository.ExistsByIdAsync(userId, ct);
        if (!userExists)
        {
            return ServiceResult<IReadOnlyList<NotificationResponse>>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var notifications = await _notificationRepository.GetByRecipientUserIdAsync(
            userId,
            isRead: null,
            pageNumber: pageNumber,
            pageSize: pageSize,
            ct: ct);

        var responses = notifications
            .Select(notification => notification.ToNotificationResponse())
            .ToList();

        return ServiceResult<IReadOnlyList<NotificationResponse>>.Ok(responses);
    }

    public async Task<ServiceResult<IReadOnlyList<NotificationResponse>>> GetUnreadNotificationsAsync(
        string userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        var userExists = await _userRepository.ExistsByIdAsync(userId, ct);
        if (!userExists)
        {
            return ServiceResult<IReadOnlyList<NotificationResponse>>.Fail(ServiceErrorType.NotFound, "User not found.");
        }

        var notifications = await _notificationRepository.GetByRecipientUserIdAsync(
            userId,
            isRead: false,
            pageNumber: pageNumber,
            pageSize: pageSize,
            ct: ct);

        var responses = notifications
            .Select(notification => notification.ToNotificationResponse())
            .ToList();

        return ServiceResult<IReadOnlyList<NotificationResponse>>.Ok(responses);
    }

    public async Task<ServiceResult<NotificationResponse>> CreateNotificationAsync(
        NotificationCreateRequest request,
        CancellationToken ct = default)
    {
        var recipientExists = await _userRepository.ExistsByIdAsync(request.RecipientUserId, ct);
        var senderExists = await _userRepository.ExistsByIdAsync(request.SenderUserId, ct);
        if (!recipientExists || !senderExists)
        {
            return ServiceResult<NotificationResponse>.Fail(ServiceErrorType.NotFound, "Recipient or sender was not found.");
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

        await _notificationRepository.AddAsync(notification, ct);

        return ServiceResult<NotificationResponse>.Ok(notification.ToNotificationResponse());
    }

    public async Task<ServiceResult<NotificationResponse>> MarkAsReadAsync(
        string notificationId,
        CancellationToken ct = default)
    {
        var markedAsRead = await _notificationRepository.MarkAsReadAsync(notificationId, ct);
        if (!markedAsRead)
        {
            return ServiceResult<NotificationResponse>.Fail(ServiceErrorType.NotFound, "Notification not found.");
        }

        var updatedNotification = await _notificationRepository.GetByIdAsync(notificationId, ct);
        if (updatedNotification is null)
        {
            return ServiceResult<NotificationResponse>.Fail(ServiceErrorType.NotFound, "Notification not found.");
        }

        return ServiceResult<NotificationResponse>.Ok(updatedNotification.ToNotificationResponse());
    }

    public async Task<ServiceResult<string>> DeleteNotificationAsync(
        string notificationId,
        CancellationToken ct = default)
    {
        var deleted = await _notificationRepository.DeleteAsync(notificationId, ct);
        if (!deleted)
        {
            return ServiceResult<string>.Fail(ServiceErrorType.NotFound, "Notification not found.");
        }

        return ServiceResult<string>.Ok("Notification deleted.");
    }
}
