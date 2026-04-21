import api, { unwrapApiResponse } from './api';
import type { ApiResponse, Post, User } from '../types';

export interface UserUpdateRequest {
  firstName?: string | null;
  lastName?: string | null;
  profilePicture?: string | null;
  bio?: string | null;
  isActive?: boolean | null;
}

export const getUsers = async () => {
  const response = await api.get<ApiResponse<User[]>>('/users');
  return unwrapApiResponse(response.data);
};

export const searchUsers = async (query: string, pageNumber = 1, pageSize = 20) => {
  const response = await api.get<ApiResponse<User[]>>('/users/search', {
    params: { query, pageNumber, pageSize }
  });
  return unwrapApiResponse(response.data);
};

export const getUserById = async (userId: string) => {
  const response = await api.get<ApiResponse<User>>(`/users/${userId}`);
  return unwrapApiResponse(response.data);
};

export const updateUser = async (userId: string, payload: UserUpdateRequest) => {
  const response = await api.put<ApiResponse<User>>(`/users/${userId}`, payload);
  return unwrapApiResponse(response.data);
};

export const getUserPosts = async (userId: string) => {
  const response = await api.get<ApiResponse<Post[]>>(`/users/${userId}/posts`);
  return unwrapApiResponse(response.data);
};
