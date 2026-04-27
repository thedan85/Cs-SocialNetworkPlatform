import axios, { AxiosError, AxiosRequestConfig, AxiosResponse } from 'axios';
import type { ApiResponse, AuthTokenResponse } from '../types';

const apiBaseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5245/api';

const api = axios.create({
  baseURL: apiBaseUrl,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
});

const refreshClient = axios.create({
  baseURL: apiBaseUrl,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
});

const tokenStorageKey = 'token';
const tokenExpiresAtKey = 'token_expires_at';
const rolesStorageKey = 'roles';
const userStorageKey = 'user';
const refreshThresholdMs = 60_000;
const authEndpoints = ['/auth/login', '/auth/register', '/auth/refresh-token'];

const isAuthEndpoint = (url?: string) => {
  if (!url) return false;
  return authEndpoints.some((endpoint) => url.includes(endpoint));
};

const storeAccessToken = (token: string, expiresAt?: string, roles?: string[]) => {
  localStorage.setItem(tokenStorageKey, token);
  if (expiresAt) {
    localStorage.setItem(tokenExpiresAtKey, expiresAt);
  }
  if (roles) {
    localStorage.setItem(rolesStorageKey, JSON.stringify(roles));
  }
};

const clearStoredAuth = () => {
  localStorage.removeItem(tokenStorageKey);
  localStorage.removeItem(tokenExpiresAtKey);
  localStorage.removeItem(userStorageKey);
  localStorage.removeItem(rolesStorageKey);
};

const getTokenExpiry = () => {
  const expiresAt = localStorage.getItem(tokenExpiresAtKey);
  if (!expiresAt) return null;
  const time = Date.parse(expiresAt);
  if (Number.isNaN(time)) return null;
  return time;
};

const isTokenExpiringSoon = () => {
  const expiry = getTokenExpiry();
  if (!expiry) return false;
  return expiry - Date.now() <= refreshThresholdMs;
};

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

const requestTokenRefresh = async () => {
  const response = await refreshClient.post<ApiResponse<AuthTokenResponse>>('/auth/refresh-token');
  const data = unwrapApiResponse(response.data);
  storeAccessToken(data.token.accessToken, data.token.expiresAt, data.token.roles);
  return data.token.accessToken;
};

let refreshPromise: Promise<string | null> | null = null;

const refreshAccessToken = async () => {
  if (!refreshPromise) {
    refreshPromise = requestTokenRefresh()
      .catch(() => null)
      .finally(() => {
        refreshPromise = null;
      });
  }

  return refreshPromise;
};

type RetryableRequestConfig = AxiosRequestConfig & { _retry?: boolean };

// Interceptor to automatically inject token into header (Requirement F2)
api.interceptors.request.use(async (config: AxiosRequestConfig) => {
  if (isAuthEndpoint(config.url)) {
    return config;
  }

  let token = localStorage.getItem(tokenStorageKey);
  if (token && isTokenExpiringSoon()) {
    const refreshedToken = await refreshAccessToken();
    if (refreshedToken) {
      token = refreshedToken;
    }
  }

  if (token) {
    config.headers = config.headers ?? {};
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Interceptor for global error handling (Requirement F2)
api.interceptors.response.use(
  (response: AxiosResponse) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as RetryableRequestConfig | undefined;

    if (
      error.response?.status === 401
      && originalRequest
      && !originalRequest._retry
      && !isAuthEndpoint(originalRequest.url)
    ) {
      originalRequest._retry = true;
      const refreshedToken = await refreshAccessToken();
      if (refreshedToken) {
        originalRequest.headers = originalRequest.headers ?? {};
        originalRequest.headers.Authorization = `Bearer ${refreshedToken}`;
        return api(originalRequest);
      }
    }

    if (error.response?.status === 401 && !isAuthEndpoint(originalRequest?.url)) {
      clearStoredAuth();
      window.location.href = '/login';
    }

    return Promise.reject(error);
  }
);

export default api;
