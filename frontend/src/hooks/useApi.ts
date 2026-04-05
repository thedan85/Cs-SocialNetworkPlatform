import { useState, useCallback } from 'react';
import api from '../services/api';

interface ApiOptions {
  headers?: Record<string, string>;
}

export const useApi = () => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const request = useCallback(async (
    method: 'get' | 'post' | 'put' | 'delete',
    url: string,
    data?: any,
    options?: ApiOptions
  ) => {
    setLoading(true);
    setError(null);
    try {
      const response = await api[method](url, data, options);
      setLoading(false);
      return response.data;
    } catch (err: any) {
      const message = err.response?.data?.message || err.message || 'An error occurred';
      setError(message);
      setLoading(false);
      throw err;
    }
  }, []);

  return { request, loading, error };
};
