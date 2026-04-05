import api from './api'
import type { ApiResponse, FriendRequest, PaginatedResponse, ProfileUpdateFormData, User } from '@/types'

const userService = {
  async getProfile(username: string): Promise<User> {
    const res = await api.get<ApiResponse<User>>(`/users/${username}`)
    return res.data.data
  },

  async updateProfile(data: ProfileUpdateFormData): Promise<User> {
    const formData = new FormData()
    formData.append('fullName', data.fullName)
    if (data.bio)       formData.append('bio', data.bio)
    if (data.avatar?.[0]) formData.append('avatar', data.avatar[0])
    if (data.cover?.[0])  formData.append('cover',  data.cover[0])

    const res = await api.put<ApiResponse<User>>('/users/me', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    return res.data.data
  },

  async searchUsers(query: string, page = 1): Promise<PaginatedResponse<User>> {
    const res = await api.get<ApiResponse<PaginatedResponse<User>>>('/users/search', {
      params: { query, page },
    })
    return res.data.data
  },

  async getFollowers(username: string, page = 1): Promise<PaginatedResponse<User>> {
    const res = await api.get<ApiResponse<PaginatedResponse<User>>>(`/users/${username}/followers`, {
      params: { page },
    })
    return res.data.data
  },

  async getFollowing(username: string, page = 1): Promise<PaginatedResponse<User>> {
    const res = await api.get<ApiResponse<PaginatedResponse<User>>>(`/users/${username}/following`, {
      params: { page },
    })
    return res.data.data
  },

  async follow(userId: string): Promise<void> {
    await api.post(`/users/${userId}/follow`)
  },

  async unfollow(userId: string): Promise<void> {
    await api.delete(`/users/${userId}/follow`)
  },

  async sendFriendRequest(userId: string): Promise<FriendRequest> {
    const res = await api.post<ApiResponse<FriendRequest>>(`/users/${userId}/friend-request`)
    return res.data.data
  },

  async acceptFriendRequest(requestId: string): Promise<void> {
    await api.put(`/friend-requests/${requestId}/accept`)
  },

  async rejectFriendRequest(requestId: string): Promise<void> {
    await api.put(`/friend-requests/${requestId}/reject`)
  },

  async getSuggestedUsers(): Promise<User[]> {
    const res = await api.get<ApiResponse<User[]>>('/users/suggested')
    return res.data.data
  },
}

export default userService
