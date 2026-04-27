import api, { unwrapApiResponse } from './api';
import type {
  ApiResponse,
  Comment,
  Like,
  Post,
  PostReport,
  PostShare
} from '../types';

export interface PostCreateRequest {
  content: string;
  imageUrl?: string | null;
  privacy?: 'Public' | 'Friends' | 'Private';
}

export interface PostUpdateRequest {
  content: string;
  imageUrl?: string | null;
  privacy?: 'Public' | 'Friends' | 'Private';
}

export interface CommentCreateRequest {
  content: string;
}

export interface PostReportCreateRequest {
  reason?: string | null;
  description?: string | null;
}

export interface PostShareCreateRequest {
  content?: string | null;
  privacy?: 'Public' | 'Friends' | 'Private';
}

export const getPosts = async (pageNumber = 1, pageSize = 20) => {
  const response = await api.get<ApiResponse<Post[]>>('/posts', {
    params: { pageNumber, pageSize }
  });
  return unwrapApiResponse(response.data);
};

export const getPostById = async (postId: string) => {
  const response = await api.get<ApiResponse<Post>>(`/posts/${postId}`);
  return unwrapApiResponse(response.data);
};

export const createPost = async (payload: PostCreateRequest) => {
  const response = await api.post<ApiResponse<Post>>('/posts', payload);
  return unwrapApiResponse(response.data);
};

export const updatePost = async (postId: string, payload: PostUpdateRequest) => {
  const response = await api.put<ApiResponse<Post>>(`/posts/${postId}`, payload);
  return unwrapApiResponse(response.data);
};

export const deletePost = async (postId: string) => {
  const response = await api.delete<ApiResponse<{ message: string }>>(`/posts/${postId}`);
  return unwrapApiResponse(response.data);
};

export const getPostComments = async (postId: string, pageNumber = 1, pageSize = 20) => {
  const response = await api.get<ApiResponse<Comment[]>>(`/posts/${postId}/comments`, {
    params: { pageNumber, pageSize }
  });
  return unwrapApiResponse(response.data);
};

export const createComment = async (postId: string, payload: CommentCreateRequest) => {
  const response = await api.post<ApiResponse<Comment>>(`/posts/${postId}/comments`, payload);
  return unwrapApiResponse(response.data);
};

export const deleteComment = async (postId: string, commentId: string) => {
  const response = await api.delete<ApiResponse<{ message: string }>>(
    `/posts/${postId}/comments/${commentId}`
  );
  return unwrapApiResponse(response.data);
};

export const likePost = async (postId: string) => {
  const response = await api.post<ApiResponse<Like>>(`/posts/${postId}/likes`, {});
  return unwrapApiResponse(response.data);
};

export const sharePost = async (postId: string, payload: PostShareCreateRequest) => {
  const response = await api.post<ApiResponse<PostShare>>(`/posts/${postId}/shares`, payload);
  return unwrapApiResponse(response.data);
};

export const unsharePost = async (postId: string) => {
  const response = await api.delete<ApiResponse<{ message: string }>>(`/posts/${postId}/shares`);
  return unwrapApiResponse(response.data);
};

export const unlikePost = async (postId: string) => {
  const response = await api.delete<ApiResponse<{ message: string }>>(`/posts/${postId}/likes`);
  return unwrapApiResponse(response.data);
};

export const reportPost = async (postId: string, payload: PostReportCreateRequest) => {
  const response = await api.post<ApiResponse<PostReport>>(`/posts/${postId}/report`, payload);
  return unwrapApiResponse(response.data);
};
