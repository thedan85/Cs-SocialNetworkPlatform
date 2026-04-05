import { useRef } from 'react'
import { Link } from 'react-router-dom'
import { useNotifications } from '@/context/NotificationContext'
import { useClickOutside } from '@/hooks/useUtils'
import { Avatar } from '@/components/common/Avatar'
import { Badge } from '@/components/common/index'
import { timeAgo } from '@/utils/helpers'
import type { Notification } from '@/types'
import { useState } from 'react'

// ─── NotificationItem ─────────────────────────────────────────────────────────
const notifLabels: Record<Notification['type'], string> = {
  like:           'liked your post',
  comment:        'commented on your post',
  follow:         'started following you',
  friend_request: 'sent you a friend request',
  mention:        'mentioned you',
  share:          'shared your post',
}

interface NotificationItemProps {
  notification: Notification
  onRead: (id: string) => void
}

export function NotificationItem({ notification: n, onRead }: NotificationItemProps) {
  return (
    <div
      onClick={() => !n.isRead && onRead(n.id)}
      className={`flex items-start gap-3 px-4 py-3 hover:bg-slate-50 transition-colors cursor-pointer
        ${!n.isRead ? 'bg-primary-50/60' : ''}`}
    >
      <Avatar
        src={n.sender.avatarUrl}
        username={n.sender.username}
        size="sm"
      />
      <div className="flex-1 min-w-0">
        <p className="text-sm text-slate-800">
          <span className="font-semibold">@{n.sender.username}</span>{' '}
          {notifLabels[n.type]}
        </p>
        <p className="text-xs text-slate-500 mt-0.5">{timeAgo(n.createdAt)}</p>
      </div>
      {!n.isRead && (
        <div className="w-2 h-2 rounded-full bg-primary-500 mt-1.5 shrink-0" />
      )}
    </div>
  )
}

// ─── NotificationBell ─────────────────────────────────────────────────────────
export function NotificationBell() {
  const { notifications, unreadCount, markAsRead, markAllAsRead, isConnected } = useNotifications()
  const [isOpen, setIsOpen] = useState(false)

  const ref = useClickOutside<HTMLDivElement>(() => setIsOpen(false))

  return (
    <div ref={ref} className="relative">
      {/* Bell Button */}
      <button
        onClick={() => setIsOpen(v => !v)}
        className="relative p-2 rounded-xl hover:bg-slate-100 text-slate-600 transition-colors"
        aria-label={`Notifications ${unreadCount > 0 ? `(${unreadCount} unread)` : ''}`}
      >
        <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
          <path strokeLinecap="round" strokeLinejoin="round"
            d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
        </svg>
        {unreadCount > 0 && (
          <span className="absolute top-1 right-1 min-w-[16px] h-4 px-1 rounded-full bg-red-500
            text-white text-[10px] font-bold flex items-center justify-center">
            {unreadCount > 99 ? '99+' : unreadCount}
          </span>
        )}
        {/* SignalR live dot */}
        {isConnected && (
          <span className="absolute bottom-1.5 right-1.5 w-1.5 h-1.5 rounded-full bg-green-500" />
        )}
      </button>

      {/* Dropdown Panel */}
      {isOpen && (
        <div className="absolute right-0 mt-2 w-80 card shadow-xl z-30 animate-slide-up overflow-hidden">
          {/* Header */}
          <div className="flex items-center justify-between px-4 py-3 border-b border-slate-100">
            <h3 className="font-semibold text-slate-900 text-sm">Notifications</h3>
            {unreadCount > 0 && (
              <button
                onClick={markAllAsRead}
                className="text-xs text-primary-600 hover:text-primary-700 font-medium transition-colors"
              >
                Mark all read
              </button>
            )}
          </div>

          {/* List */}
          <div className="max-h-80 overflow-y-auto divide-y divide-slate-50">
            {notifications.length === 0 ? (
              <p className="px-4 py-8 text-center text-sm text-slate-500">No notifications yet</p>
            ) : (
              notifications.slice(0, 20).map(n => (
                <NotificationItem key={n.id} notification={n} onRead={markAsRead} />
              ))
            )}
          </div>

          {/* Footer */}
          <div className="px-4 py-2 border-t border-slate-100">
            <Link
              to="/notifications"
              className="block text-center text-xs text-primary-600 hover:text-primary-700 font-medium py-1"
              onClick={() => setIsOpen(false)}
            >
              View all notifications
            </Link>
          </div>
        </div>
      )}
    </div>
  )
}
