import api, { unwrapApiResponse } from './api';
import type { ApiResponse, FriendRelationship, Friendship } from '../types';

export interface FriendRequestCreateRequest {
  addresseeUserId: string;
}

export const createFriendRequest = async (payload: FriendRequestCreateRequest) => {
  const response = await api.post<ApiResponse<Friendship>>('/friends/requests', payload);
  return unwrapApiResponse(response.data);
};

export const acceptFriendRequest = async (friendshipId: string) => {
  const response = await api.put<ApiResponse<Friendship>>(`/friends/requests/${friendshipId}/accept`, {});
  return unwrapApiResponse(response.data);
};

export const rejectFriendRequest = async (friendshipId: string) => {
  const response = await api.put<ApiResponse<Friendship>>(`/friends/requests/${friendshipId}/reject`, {});
  return unwrapApiResponse(response.data);
};

export const getFriends = async (userId: string, pageNumber = 1, pageSize = 50) => {
  const response = await api.get<ApiResponse<Friendship[]>>(`/friends/${userId}`, {
    params: { pageNumber, pageSize }
  });
  return unwrapApiResponse(response.data);
};

export const getPendingRequests = async (userId: string, pageNumber = 1, pageSize = 50) => {
  const response = await api.get<ApiResponse<Friendship[]>>(`/friends/requests/${userId}`, {
    params: { pageNumber, pageSize }
  });
  return unwrapApiResponse(response.data);
};

export const getFriendRelationship = async (userId: string) => {
  const response = await api.get<ApiResponse<FriendRelationship>>(`/friends/relationship/${userId}`);
  return unwrapApiResponse(response.data);
};

export const removeFriend = async (friendshipId: string) => {
  const response = await api.delete<ApiResponse<{ message: string }>>(`/friends/${friendshipId}`);
  return unwrapApiResponse(response.data);
};
