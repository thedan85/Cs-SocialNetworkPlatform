import api, { unwrapApiResponse } from './api';
import type { ApiResponse, Story } from '../types';

export interface StoryCreateRequest {
  content: string;
  imageUrl?: string | null;
  expiresAt?: string | null;
}

export const getStories = async (pageNumber = 1, pageSize = 50) => {
  const response = await api.get<ApiResponse<Story[]>>('/stories', {
    params: { pageNumber, pageSize }
  });
  return unwrapApiResponse(response.data);
};

export const getStoryById = async (storyId: string) => {
  const response = await api.get<ApiResponse<Story>>(`/stories/${storyId}`);
  return unwrapApiResponse(response.data);
};

export const getStoriesForUser = async (userId: string, pageNumber = 1, pageSize = 50) => {
  const response = await api.get<ApiResponse<Story[]>>(`/stories/user/${userId}`, {
    params: { pageNumber, pageSize }
  });
  return unwrapApiResponse(response.data);
};

export const createStory = async (payload: StoryCreateRequest) => {
  const response = await api.post<ApiResponse<Story>>('/stories', payload);
  return unwrapApiResponse(response.data);
};

export const deleteStory = async (storyId: string) => {
  const response = await api.delete<ApiResponse<{ message: string }>>(`/stories/${storyId}`);
  return unwrapApiResponse(response.data);
};
