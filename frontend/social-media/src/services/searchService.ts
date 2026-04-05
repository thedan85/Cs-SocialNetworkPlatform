import api from './api'
import type { ApiResponse, PaginatedResponse, Post, SearchParams, User } from '@/types'

const searchService = {
  async searchAll(params: SearchParams): Promise<{ users: User[]; posts: Post[] }> {
    const res = await api.get<ApiResponse<{ users: User[]; posts: Post[] }>>('/search', { params })
    return res.data.data
  },

  async searchUsers(query: string, page = 1, pageSize = 10): Promise<PaginatedResponse<User>> {
    const res = await api.get<ApiResponse<PaginatedResponse<User>>>('/search/users', {
      params: { query, page, pageSize },
    })
    return res.data.data
  },

  async searchPosts(query: string, page = 1, pageSize = 10): Promise<PaginatedResponse<Post>> {
    const res = await api.get<ApiResponse<PaginatedResponse<Post>>>('/search/posts', {
      params: { query, page, pageSize },
    })
    return res.data.data
  },
}

export default searchService
