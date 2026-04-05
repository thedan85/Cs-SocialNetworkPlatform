import api from './api'
import type { ApiResponse, Comment, PaginatedResponse, Post, PostFormData } from '@/types'

const postService = {
  async getFeed(page = 1, pageSize = 10): Promise<PaginatedResponse<Post>> {
    const res = await api.get<ApiResponse<PaginatedResponse<Post>>>('/posts/feed', {
      params: { page, pageSize },
    })
    return res.data.data
  },

  async getUserPosts(username: string, page = 1, pageSize = 10): Promise<PaginatedResponse<Post>> {
    const res = await api.get<ApiResponse<PaginatedResponse<Post>>>(`/posts/user/${username}`, {
      params: { page, pageSize },
    })
    return res.data.data
  },

  async getPost(postId: string): Promise<Post> {
    const res = await api.get<ApiResponse<Post>>(`/posts/${postId}`)
    return res.data.data
  },

  async createPost(data: PostFormData): Promise<Post> {
    const formData = new FormData()
    formData.append('content', data.content)
    if (data.image?.[0]) formData.append('image', data.image[0])

    const res = await api.post<ApiResponse<Post>>('/posts', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    return res.data.data
  },

  async deletePost(postId: string): Promise<void> {
    await api.delete(`/posts/${postId}`)
  },

  async likePost(postId: string): Promise<void> {
    await api.post(`/posts/${postId}/like`)
  },

  async unlikePost(postId: string): Promise<void> {
    await api.delete(`/posts/${postId}/like`)
  },

  async savePost(postId: string): Promise<void> {
    await api.post(`/posts/${postId}/save`)
  },

  async unsavePost(postId: string): Promise<void> {
    await api.delete(`/posts/${postId}/save`)
  },

  async getComments(postId: string, page = 1): Promise<PaginatedResponse<Comment>> {
    const res = await api.get<ApiResponse<PaginatedResponse<Comment>>>(
      `/posts/${postId}/comments`,
      { params: { page } }
    )
    return res.data.data
  },

  async addComment(postId: string, content: string): Promise<Comment> {
    const res = await api.post<ApiResponse<Comment>>(`/posts/${postId}/comments`, { content })
    return res.data.data
  },

  async deleteComment(postId: string, commentId: string): Promise<void> {
    await api.delete(`/posts/${postId}/comments/${commentId}`)
  },
}

export default postService
