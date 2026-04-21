import axios, { AxiosError, AxiosRequestConfig, AxiosResponse } from 'axios';
import type { ApiResponse } from '../types';

const apiBaseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5245/api';

const api = axios.create({
  baseURL: apiBaseUrl,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor to automatically inject token into header (Requirement F2)
api.interceptors.request.use((config: AxiosRequestConfig) => {
  const token = localStorage.getItem('token');
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Interceptor for global error handling (Requirement F2)
api.interceptors.response.use(
  (response: AxiosResponse) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      // Handle token expiration - logout user
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;

export const unwrapApiResponse = <T>(response: ApiResponse<T>) => {
  if (!response.success) {
    const message = response.errors?.[0] || 'Request failed.';
    throw new Error(message);
  }

  if (response.data === undefined || response.data === null) {
    throw new Error('Response data is missing.');
  }

  return response.data;
};
