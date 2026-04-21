import { useEffect, useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import type { Notification } from '../types';
import { createNotificationHubConnection } from '../services/notificationHub';
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

  useEffect(() => {
    if (!user) {
      return;
    }

    const token = localStorage.getItem('token');
    if (!token) {
      return;
    }

    const connection = createNotificationHubConnection(token);

    connection.on('notification:created', (incoming: Notification) => {
      setItems((current) => {
        const exists = current.some((item) => item.notificationId === incoming.notificationId);
        if (exists) {
          return current;
        }

        return [incoming, ...current];
      });
    });

    connection.start().catch(() => undefined);

    return () => {
      connection.stop().catch(() => undefined);
    };
  }, [user?.userId]);

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
    return <div className="text-center text-slate-600 dark:text-slate-400">Sign in to view notifications.</div>;
  }

  return (
    <div className="max-w-3xl mx-auto space-y-4">
      <h1 className="text-2xl font-bold text-slate-900 tracking-tight dark:text-slate-100">Notifications</h1>

      {loading && <div className="text-slate-600 dark:text-slate-400">Loading notifications...</div>}
      {error && (
        <div className="rounded-xl border border-rose-200/70 bg-rose-50/70 px-4 py-3 text-sm text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300">
          {error}
        </div>
      )}

      {!loading && items.length === 0 && (
        <div className="rounded-2xl border border-white/60 bg-white/70 px-4 py-6 text-center text-slate-600 backdrop-blur-xl dark:border-slate-800/60 dark:bg-slate-900/60 dark:text-slate-300">
          No notifications yet.
        </div>
      )}

      <div className="space-y-3">
        {items.map((item) => (
          <div
            key={item.notificationId}
            className={`rounded-2xl border px-4 py-3 shadow-[0_12px_30px_rgba(15,23,42,0.08)] backdrop-blur-xl ${
              item.isRead
                ? 'border-white/60 bg-white/70 dark:border-slate-800/60 dark:bg-slate-900/60'
                : 'border-cyan-200/70 bg-cyan-50/70 dark:border-cyan-500/30 dark:bg-cyan-500/10'
            }`}
          >
            <div className="flex items-start justify-between gap-4">
              <div>
                <p className="text-sm font-semibold text-slate-900 dark:text-slate-100">
                  {item.type || 'Notification'}
                </p>
                <p className="text-sm text-slate-600 dark:text-slate-300">{item.content || 'No details provided.'}</p>
                <p className="text-xs text-slate-400 mt-1 dark:text-slate-500">
                  {new Date(item.createdAt).toLocaleString()}
                </p>
              </div>
              <div className="flex items-center gap-2">
                {!item.isRead && (
                  <button
                    onClick={() => handleMarkRead(item.notificationId)}
                    className="rounded-md border border-cyan-200/70 bg-white/80 px-3 py-1 text-xs font-medium text-cyan-700 hover:bg-cyan-50 dark:border-cyan-500/30 dark:bg-slate-900/60 dark:text-cyan-200 dark:hover:bg-cyan-500/10"
                  >
                    Mark read
                  </button>
                )}
                <button
                  onClick={() => handleDelete(item.notificationId)}
                  className="rounded-md border border-rose-200/70 bg-white/80 px-3 py-1 text-xs font-medium text-rose-600 hover:bg-rose-50 dark:border-rose-500/30 dark:bg-slate-900/60 dark:text-rose-300 dark:hover:bg-rose-500/10"
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
