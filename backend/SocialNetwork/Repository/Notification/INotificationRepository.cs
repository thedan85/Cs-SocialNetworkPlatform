using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(string notificationId, CancellationToken ct = default);

    Task<IReadOnlyList<Notification>> GetByRecipientUserIdAsync(
        string recipientUserId,
        bool? isRead,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    Task<int> CountByRecipientUserIdAsync(
        string recipientUserId,
        bool? isRead = null,
        CancellationToken ct = default);

    Task AddAsync(Notification notification, CancellationToken ct = default);

    Task<Notification?> MarkAsReadAsync(string notificationId, CancellationToken ct = default);

    Task<int> MarkAllAsReadAsync(string recipientUserId, CancellationToken ct = default);

    Task<bool> DeleteAsync(string notificationId, CancellationToken ct = default);
}
