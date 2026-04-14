import { useEffect, useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import type { Notification } from '../types';
import { deleteNotification, getNotifications, markNotificationRead } from '../services/notifications';

const Notifications = () => {
  const { user } = useAuth();
  const [items, setItems] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const loadNotifications = async () => {
    if (!user) return;
    setLoading(true);
    setError(null);
    try {
      const data = await getNotifications(user.userId);
      setItems(data);
    } catch (err: any) {
      setError(err.message || 'Failed to load notifications.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadNotifications();
  }, [user]);

  const handleMarkRead = async (notificationId: string) => {
    try {
      const updated = await markNotificationRead(notificationId);
      setItems((current) =>
        current.map((item) => (item.notificationId === updated.notificationId ? updated : item))
      );
    } catch (err) {
      alert('Unable to mark notification as read.');
    }
  };

  const handleDelete = async (notificationId: string) => {
    try {
      await deleteNotification(notificationId);
      setItems((current) => current.filter((item) => item.notificationId !== notificationId));
    } catch (err) {
      alert('Unable to delete notification.');
    }
  };

  if (!user) {
    return <div className="text-center text-gray-600">Sign in to view notifications.</div>;
  }

  return (
    <div className="max-w-3xl mx-auto space-y-4">
      <h1 className="text-2xl font-bold text-gray-800">Notifications</h1>

      {loading && <div className="text-gray-600">Loading notifications...</div>}
      {error && (
        <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {error}
        </div>
      )}

      {!loading && items.length === 0 && (
        <div className="rounded-lg border border-gray-200 bg-white px-4 py-6 text-center text-gray-600">
          No notifications yet.
        </div>
      )}

      <div className="space-y-3">
        {items.map((item) => (
          <div
            key={item.notificationId}
            className={`rounded-lg border px-4 py-3 shadow-sm ${
              item.isRead ? 'border-gray-200 bg-white' : 'border-blue-200 bg-blue-50'
            }`}
          >
            <div className="flex items-start justify-between gap-4">
              <div>
                <p className="text-sm font-semibold text-gray-800">
                  {item.type || 'Notification'}
                </p>
                <p className="text-sm text-gray-600">{item.content || 'No details provided.'}</p>
                <p className="text-xs text-gray-400 mt-1">
                  {new Date(item.createdAt).toLocaleString()}
                </p>
              </div>
              <div className="flex items-center gap-2">
                {!item.isRead && (
                  <button
                    onClick={() => handleMarkRead(item.notificationId)}
                    className="rounded-md border border-blue-200 bg-white px-3 py-1 text-xs font-medium text-blue-600 hover:bg-blue-50"
                  >
                    Mark read
                  </button>
                )}
                <button
                  onClick={() => handleDelete(item.notificationId)}
                  className="rounded-md border border-red-200 bg-white px-3 py-1 text-xs font-medium text-red-600 hover:bg-red-50"
                >
                  Delete
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Notifications;
