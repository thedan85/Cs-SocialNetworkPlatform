import api, { unwrapApiResponse } from './api';
import type { ApiResponse, Notification } from '../types';

export interface NotificationCreateRequest {
  recipientUserId: string;
  senderUserId?: string | null;
  type?: string | null;
  content?: string | null;
}

export const getNotifications = async (userId: string, pageNumber = 1, pageSize = 50) => {
  const response = await api.get<ApiResponse<Notification[]>>(`/notifications/user/${userId}`, {
    params: { pageNumber, pageSize }
  });
  return unwrapApiResponse(response.data);
};

export const getUnreadNotifications = async (userId: string, pageNumber = 1, pageSize = 50) => {
  const response = await api.get<ApiResponse<Notification[]>>(`/notifications/user/${userId}/unread`, {
    params: { pageNumber, pageSize }
  });
  return unwrapApiResponse(response.data);
};

export const createNotification = async (payload: NotificationCreateRequest) => {
  const response = await api.post<ApiResponse<Notification>>('/notifications', payload);
  return unwrapApiResponse(response.data);
};

export const markNotificationRead = async (notificationId: string) => {
  const response = await api.put<ApiResponse<Notification>>(`/notifications/${notificationId}/read`, {});
  return unwrapApiResponse(response.data);
};

export const deleteNotification = async (notificationId: string) => {
  const response = await api.delete<ApiResponse<{ message: string }>>(`/notifications/${notificationId}`);
  return unwrapApiResponse(response.data);
};
