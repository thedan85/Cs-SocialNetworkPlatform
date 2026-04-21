import api, { unwrapApiResponse } from './api';
import type { ApiResponse, HashtagSearchResult, HashtagTrendingResult } from '../types';

export const searchHashtags = async (
  query: string,
  pageNumber = 1,
  pageSize = 5,
  postsPerHashtag = 3
) => {
  const response = await api.get<ApiResponse<HashtagSearchResult[]>>('/hashtags/search', {
    params: { query, pageNumber, pageSize, postsPerHashtag }
  });
  return unwrapApiResponse(response.data);
};

export const getTrendingHashtags = async (pageNumber = 1, pageSize = 6) => {
  const response = await api.get<ApiResponse<HashtagTrendingResult[]>>('/hashtags/trending', {
    params: { pageNumber, pageSize }
  });
  return unwrapApiResponse(response.data);
};
