import api, { unwrapApiResponse } from './api';
import type { ApiResponse, AuthTokenResponse, AuthUser, User } from '../types';

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  password: string;
  bio?: string | null;
}

export interface LoginRequest {
  userNameOrEmail: string;
  password: string;
}

export const register = async (payload: RegisterRequest) => {
  const response = await api.post<ApiResponse<AuthUser>>('/auth/register', payload);
  return unwrapApiResponse(response.data);
};

export const login = async (payload: LoginRequest) => {
  const response = await api.post<ApiResponse<AuthTokenResponse>>('/auth/login', payload);
  return unwrapApiResponse(response.data);
};

export const toUser = (authUser: AuthUser): User => {
  return {
    userId: authUser.userId,
    userName: authUser.userName,
    email: authUser.email,
    firstName: authUser.firstName ?? null,
    lastName: authUser.lastName ?? null,
    bio: authUser.bio ?? null
  };
};
