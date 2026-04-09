using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Model;

namespace SocialNetwork.Repository;

public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _dbContext;

    public NotificationRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Notification?> GetByIdAsync(string notificationId, CancellationToken ct = default)
    {
        return _dbContext.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId, ct);
    }

    public async Task<IReadOnlyList<Notification>> GetByRecipientUserIdAsync(
        string recipientUserId,
        bool? isRead,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = _dbContext.Notifications
            .AsNoTracking()
            .Where(n => n.RecipientUserId == recipientUserId);

        if (isRead.HasValue)
        {
            query = query.Where(n => n.IsRead == isRead.Value);
        }

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return notifications;
    }

    public Task<int> CountByRecipientUserIdAsync(
        string recipientUserId,
        bool? isRead = null,
        CancellationToken ct = default)
    {
        var query = _dbContext.Notifications.Where(n => n.RecipientUserId == recipientUserId);

        if (isRead.HasValue)
        {
            query = query.Where(n => n.IsRead == isRead.Value);
        }

        return query.CountAsync(ct);
    }

    public async Task AddAsync(Notification notification, CancellationToken ct = default)
    {
        await _dbContext.Notifications.AddAsync(notification, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> MarkAsReadAsync(string notificationId, CancellationToken ct = default)
    {
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId, ct);
        if (notification is null)
        {
            return false;
        }

        if (notification.IsRead)
        {
            return true;
        }

        notification.IsRead = true;
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }

    public async Task<int> MarkAllAsReadAsync(string recipientUserId, CancellationToken ct = default)
    {
        var unreadNotifications = await _dbContext.Notifications
            .Where(n => n.RecipientUserId == recipientUserId && !n.IsRead)
            .ToListAsync(ct);

        if (unreadNotifications.Count == 0)
        {
            return 0;
        }

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
        }

        await _dbContext.SaveChangesAsync(ct);
        return unreadNotifications.Count;
    }
}
