import api, { unwrapApiResponse } from './api';
import type { ApiResponse, PostReportDetail } from '../types';

export const getPendingPostReports = async (pageNumber = 1, pageSize = 20) => {
  const response = await api.get<ApiResponse<PostReportDetail[]>>('/post-reports/pending', {
    params: { pageNumber, pageSize }
  });
  return unwrapApiResponse(response.data);
};

export const reviewPostReport = async (postReportId: string, reviewed = true) => {
  const response = await api.put<ApiResponse<{ message: string }>>(
    `/post-reports/${postReportId}/review`,
    { reviewed }
  );
  return unwrapApiResponse(response.data);
};
