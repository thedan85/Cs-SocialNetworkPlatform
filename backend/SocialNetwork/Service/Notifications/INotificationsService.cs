using SocialNetwork.Dtos;

namespace SocialNetwork.Service;

public interface INotificationsService
{
    Task<ServiceResult<IReadOnlyList<NotificationResponse>>> GetNotificationsAsync(
        string userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default);

    Task<ServiceResult<IReadOnlyList<NotificationResponse>>> GetUnreadNotificationsAsync(
        string userId,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default);

    Task<ServiceResult<NotificationResponse>> CreateNotificationAsync(
        string senderUserId,
        NotificationCreateRequest request,
        CancellationToken ct = default);
    Task<ServiceResult<NotificationResponse>> MarkAsReadAsync(
        string actorUserId,
        string notificationId,
        bool isAdmin,
        CancellationToken ct = default);
    Task<ServiceResult<string>> DeleteNotificationAsync(
        string actorUserId,
        string notificationId,
        bool isAdmin,
        CancellationToken ct = default);
}
