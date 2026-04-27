import { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Heart, MessageCircle, UserPlus, Bell, X } from "lucide-react";

interface Notification {
  id: string;
  type: "like" | "comment" | "follow" | "mention";
  user: {
    name: string;
    avatar: string;
  };
  message: string;
  timestamp: string;
  read: boolean;
  postImage?: string;
}

interface NotificationDropdownProps {
  isOpen: boolean;
  onClose: () => void;
}

const mockNotifications: Notification[] = [
  {
    id: "1",
    type: "like",
    user: {
      name: "Sarah Johnson",
      avatar: "https://i.pravatar.cc/150?img=5",
    },
    message: "liked your post",
    timestamp: "2m ago",
    read: false,
    postImage:
      "https://images.unsplash.com/photo-1682687220742-aba13b6e50ba?w=100&h=100&fit=crop",
  },
  {
    id: "2",
    type: "comment",
    user: {
      name: "Mike Chen",
      avatar: "https://i.pravatar.cc/150?img=12",
    },
    message: 'commented: "Amazing photo! Where was this taken?"',
    timestamp: "15m ago",
    read: false,
    postImage:
      "https://images.unsplash.com/photo-1682687220063-4742bd7fd538?w=100&h=100&fit=crop",
  },
  {
    id: "3",
    type: "follow",
    user: {
      name: "Emma Wilson",
      avatar: "https://i.pravatar.cc/150?img=9",
    },
    message: "started following you",
    timestamp: "1h ago",
    read: false,
  },
  {
    id: "4",
    type: "like",
    user: {
      name: "Alex Martinez",
      avatar: "https://i.pravatar.cc/150?img=33",
    },
    message: "and 12 others liked your post",
    timestamp: "3h ago",
    read: true,
    postImage:
      "https://images.unsplash.com/photo-1682687220208-22d7a2543e88?w=100&h=100&fit=crop",
  },
  {
    id: "5",
    type: "comment",
    user: {
      name: "David Kim",
      avatar: "https://i.pravatar.cc/150?img=15",
    },
    message: 'replied to your comment: "Thanks for sharing!"',
    timestamp: "5h ago",
    read: true,
  },
  {
    id: "6",
    type: "follow",
    user: {
      name: "Lisa Anderson",
      avatar: "https://i.pravatar.cc/150?img=20",
    },
    message: "started following you",
    timestamp: "1d ago",
    read: true,
  },
];

const notificationIcons = {
  like: Heart,
  comment: MessageCircle,
  follow: UserPlus,
  mention: Bell,
};

const notificationColors = {
  like: "text-red-500 bg-red-50",
  comment: "text-blue-500 bg-blue-50",
  follow: "text-purple-500 bg-purple-50",
  mention: "text-orange-500 bg-orange-50",
};

export function NotificationDropdown({
  isOpen,
  onClose,
}: NotificationDropdownProps) {
  const [notifications, setNotifications] = useState(mockNotifications);
  const unreadCount = notifications.filter((n) => !n.read).length;

  const markAllAsRead = () => {
    setNotifications((prev) => prev.map((n) => ({ ...n, read: true })));
  };

  const markAsRead = (id: string) => {
    setNotifications((prev) =>
      prev.map((n) => (n.id === id ? { ...n, read: true } : n)),
    );
  };

  return (
    <AnimatePresence>
      {isOpen && (
        <>
          {/* Backdrop for mobile */}
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="fixed inset-0 bg-black/20 backdrop-blur-sm z-40 md:hidden"
            onClick={onClose}
          />

          {/* Dropdown */}
          <motion.div
            initial={{ opacity: 0, y: -10, scale: 0.95 }}
            animate={{ opacity: 1, y: 0, scale: 1 }}
            exit={{ opacity: 0, y: -10, scale: 0.95 }}
            transition={{ duration: 0.2, ease: [0.19, 1, 0.22, 1] }}
            className="absolute top-14 right-0 md:right-4 w-full md:w-[420px] bg-white rounded-2xl shadow-2xl border border-gray-100 overflow-hidden z-50"
            style={{ fontFamily: "'DM Sans', sans-serif" }}
          >
            {/* Header */}
            <div className="p-5 border-b border-gray-100 bg-gradient-to-r from-blue-50 to-purple-50">
              <div className="flex items-center justify-between mb-2">
                <h3
                  className="font-bold text-xl text-gray-900"
                  style={{ fontFamily: "'Sora', sans-serif" }}
                >
                  Notifications
                </h3>
                <button
                  onClick={onClose}
                  className="md:hidden p-2 hover:bg-white/50 rounded-full transition-colors duration-200"
                >
                  <X className="w-5 h-5 text-gray-600" />
                </button>
              </div>
              <div className="flex items-center justify-between">
                <p className="text-sm text-gray-600">
                  You have{" "}
                  <span className="font-semibold text-blue-600">
                    {unreadCount}
                  </span>{" "}
                  unread notifications
                </p>
                {unreadCount > 0 && (
                  <button
                    onClick={markAllAsRead}
                    className="text-xs font-semibold text-blue-600 hover:text-blue-700 transition-colors duration-200"
                  >
                    Mark all read
                  </button>
                )}
              </div>
            </div>

            {/* Notifications List */}
            <div className="max-h-[500px] overflow-y-auto">
              {notifications.length === 0 ? (
                <div className="p-12 text-center">
                  <div className="w-16 h-16 mx-auto mb-4 bg-gray-100 rounded-full flex items-center justify-center">
                    <Bell className="w-8 h-8 text-gray-400" />
                  </div>
                  <p className="text-gray-500 font-medium">
                    No notifications yet
                  </p>
                  <p className="text-sm text-gray-400 mt-1">
                    When you get notifications, they'll show up here
                  </p>
                </div>
              ) : (
                <div className="divide-y divide-gray-50">
                  {notifications.map((notification, index) => {
                    const Icon = notificationIcons[notification.type];
                    const colorClass = notificationColors[notification.type];

                    return (
                      <motion.div
                        key={notification.id}
                        initial={{ opacity: 0, x: -20 }}
                        animate={{ opacity: 1, x: 0 }}
                        transition={{ delay: index * 0.05 }}
                        onClick={() => markAsRead(notification.id)}
                        className={`p-4 flex gap-3 cursor-pointer transition-all duration-200 hover:bg-blue-50/50 ${
                          !notification.read ? "bg-blue-50/30" : "bg-white"
                        }`}
                      >
                        {/* User Avatar with Icon Badge */}
                        <div className="relative flex-shrink-0">
                          <img
                            src={notification.user.avatar}
                            alt={notification.user.name}
                            className="w-12 h-12 rounded-full object-cover ring-2 ring-white"
                          />
                          <div
                            className={`absolute -bottom-1 -right-1 w-6 h-6 rounded-full ${colorClass} flex items-center justify-center ring-2 ring-white shadow-sm`}
                          >
                            <Icon className="w-3.5 h-3.5" />
                          </div>
                        </div>

                        {/* Content */}
                        <div className="flex-1 min-w-0">
                          <p className="text-sm text-gray-700 leading-relaxed">
                            <span
                              className="font-semibold text-gray-900"
                              style={{ fontFamily: "'Sora', sans-serif" }}
                            >
                              {notification.user.name}
                            </span>{" "}
                            {notification.message}
                          </p>
                          <div className="flex items-center gap-2 mt-1">
                            <p className="text-xs text-gray-400">
                              {notification.timestamp}
                            </p>
                            {!notification.read && (
                              <span className="w-1.5 h-1.5 rounded-full bg-blue-600"></span>
                            )}
                          </div>
                        </div>

                        {/* Post Thumbnail */}
                        {notification.postImage && (
                          <img
                            src={notification.postImage}
                            alt="Post"
                            className="w-12 h-12 rounded-lg object-cover flex-shrink-0 ring-1 ring-gray-100"
                          />
                        )}
                      </motion.div>
                    );
                  })}
                </div>
              )}
            </div>

            {/* Footer */}
            {notifications.length > 0 && (
              <div className="p-4 border-t border-gray-100 bg-gray-50/50">
                <button className="w-full text-center text-sm font-semibold text-blue-600 hover:text-blue-700 py-2 hover:bg-blue-50 rounded-lg transition-all duration-200">
                  View all notifications
                </button>
              </div>
            )}
          </motion.div>
        </>
      )}
    </AnimatePresence>
  );
}
