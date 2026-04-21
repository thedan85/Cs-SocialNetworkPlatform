import { useState, useCallback } from 'react';
import api, { unwrapApiResponse } from '../services/api';
import type { ApiResponse } from '../types';

interface ApiOptions {
  headers?: Record<string, string>;
  params?: Record<string, string | number | boolean | undefined>;
}

export const useApi = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const request = useCallback(async <T>(
    method: 'get' | 'post' | 'put' | 'delete',
    url: string,
    data?: any,
    options?: ApiOptions
  ) => {
    setLoading(true);
    setError(null);
    try {
      const response = await api.request<ApiResponse<T>>({
        method,
        url,
        data,
        headers: options?.headers,
        params: options?.params
      });
      const payload = unwrapApiResponse(response.data);
      setLoading(false);
      return payload;
    } catch (err: any) {
      const message = err.response?.data?.errors?.[0] || err.message || 'An error occurred';
      setError(message);
      setLoading(false);
      throw err;
    }
  }, []);

  return { request, loading, error };
};
