import api from './api'
import type { ApiResponse, Notification, PaginatedResponse } from '@/types'

const notificationService = {
  async getNotifications(page = 1): Promise<PaginatedResponse<Notification>> {
    const res = await api.get<ApiResponse<PaginatedResponse<Notification>>>('/notifications', {
      params: { page },
    })
    return res.data.data
  },

  async getUnreadCount(): Promise<number> {
    const res = await api.get<ApiResponse<{ count: number }>>('/notifications/unread-count')
    return res.data.data.count
  },

  async markAsRead(notificationId: string): Promise<void> {
    await api.put(`/notifications/${notificationId}/read`)
  },

  async markAllAsRead(): Promise<void> {
    await api.put('/notifications/read-all')
  },

  async deleteNotification(notificationId: string): Promise<void> {
    await api.delete(`/notifications/${notificationId}`)
  },
}

export default notificationService
