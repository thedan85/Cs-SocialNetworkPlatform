import api, { unwrapApiResponse } from './api';
import type { ApiResponse, HashtagSearchResult } from '../types';

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
