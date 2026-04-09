using Microsoft.EntityFrameworkCore;
using SocialNetwork.Data;
using SocialNetwork.Extensions;
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
            .ApplyPaging(pageNumber, pageSize, defaultPageSize: 50)
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

    public async Task<Notification?> MarkAsReadAsync(string notificationId, CancellationToken ct = default)
    {
        var notification = await _dbContext.Notifications
            .FirstOrDefaultAsync(n => n.NotificationId == notificationId, ct);

        if (notification is null)
        {
            return null;
        }

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await _dbContext.SaveChangesAsync(ct);
        }

        return notification;
    }

    public async Task<int> MarkAllAsReadAsync(string recipientUserId, CancellationToken ct = default)
    {
        return await _dbContext.Notifications
            .Where(n => n.RecipientUserId == recipientUserId && !n.IsRead)
            .ExecuteUpdateAsync(
                setter => setter.SetProperty(n => n.IsRead, true),
                ct);
    }

    public async Task<bool> DeleteAsync(string notificationId, CancellationToken ct = default)
    {
        var affectedRows = await _dbContext.Notifications
            .Where(entity => entity.NotificationId == notificationId)
            .ExecuteDeleteAsync(ct);

        return affectedRows > 0;
    }
}
