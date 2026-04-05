import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useRef,
  useState,
  type ReactNode,
} from 'react'
import * as signalR from '@microsoft/signalr'
import { useAuth } from './AuthContext'
import notificationService from '@/services/notificationService'
import { SIGNALR_HUB_URL, TOKEN_KEY } from '@/utils/constants'
import type { Notification } from '@/types'

// ─── Context Types ────────────────────────────────────────────────────────────
interface NotificationContextValue {
  notifications:  Notification[]
  unreadCount:    number
  isConnected:    boolean
  markAsRead:     (id: string) => Promise<void>
  markAllAsRead:  () => Promise<void>
  loadMore:       () => Promise<void>
  hasMore:        boolean
}

const NotificationContext = createContext<NotificationContextValue | null>(null)

// ─── Provider ─────────────────────────────────────────────────────────────────
export function NotificationProvider({ children }: { children: ReactNode }) {
  const { isAuthenticated } = useAuth()
  const [notifications, setNotifications] = useState<Notification[]>([])
  const [unreadCount,   setUnreadCount]   = useState(0)
  const [isConnected,   setIsConnected]   = useState(false)
  const [page,          setPage]          = useState(1)
  const [hasMore,       setHasMore]       = useState(true)
  const hubRef = useRef<signalR.HubConnection | null>(null)

  // Initial load
  useEffect(() => {
    if (!isAuthenticated) return
    notificationService.getUnreadCount().then(setUnreadCount)
    notificationService.getNotifications(1).then(res => {
      setNotifications(res.items)
      setHasMore(res.hasNextPage)
    })
  }, [isAuthenticated])

  // SignalR connection
  useEffect(() => {
    if (!isAuthenticated) return

    const token = localStorage.getItem(TOKEN_KEY) ?? ''
    const hub = new signalR.HubConnectionBuilder()
      .withUrl(SIGNALR_HUB_URL, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build()

    hub.on('ReceiveNotification', (notification: Notification) => {
      setNotifications(prev => [notification, ...prev])
      setUnreadCount(c => c + 1)
    })

    hub.start()
      .then(() => setIsConnected(true))
      .catch(console.error)

    hub.onreconnected(() => setIsConnected(true))
    hub.onclose(()     => setIsConnected(false))

    hubRef.current = hub
    return () => { hub.stop() }
  }, [isAuthenticated])

  const markAsRead = useCallback(async (id: string) => {
    await notificationService.markAsRead(id)
    setNotifications(prev =>
      prev.map(n => n.id === id ? { ...n, isRead: true } : n)
    )
    setUnreadCount(c => Math.max(0, c - 1))
  }, [])

  const markAllAsRead = useCallback(async () => {
    await notificationService.markAllAsRead()
    setNotifications(prev => prev.map(n => ({ ...n, isRead: true })))
    setUnreadCount(0)
  }, [])

  const loadMore = useCallback(async () => {
    if (!hasMore) return
    const nextPage = page + 1
    const res = await notificationService.getNotifications(nextPage)
    setNotifications(prev => [...prev, ...res.items])
    setHasMore(res.hasNextPage)
    setPage(nextPage)
  }, [hasMore, page])

  const value = useMemo<NotificationContextValue>(
    () => ({ notifications, unreadCount, isConnected, markAsRead, markAllAsRead, loadMore, hasMore }),
    [notifications, unreadCount, isConnected, markAsRead, markAllAsRead, loadMore, hasMore]
  )

  return (
    <NotificationContext.Provider value={value}>
      {children}
    </NotificationContext.Provider>
  )
}

// ─── Hook ─────────────────────────────────────────────────────────────────────
export function useNotifications(): NotificationContextValue {
  const ctx = useContext(NotificationContext)
  if (!ctx) throw new Error('useNotifications must be used within NotificationProvider')
  return ctx
}
