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

    Task<ServiceResult<NotificationResponse>> CreateNotificationAsync(NotificationCreateRequest request, CancellationToken ct = default);
    Task<ServiceResult<NotificationResponse>> MarkAsReadAsync(string notificationId, CancellationToken ct = default);
    Task<ServiceResult<string>> DeleteNotificationAsync(string notificationId, CancellationToken ct = default);
}
