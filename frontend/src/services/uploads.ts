import api, { unwrapApiResponse } from './api';
import type { ApiResponse } from '../types';

export interface UploadResponse {
  url: string;
  fileName: string;
}

export const uploadImage = async (file: File) => {
  const formData = new FormData();
  formData.append('file', file);

  const response = await api.post<ApiResponse<UploadResponse>>('/uploads/images', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  });

  return unwrapApiResponse(response.data);
};
