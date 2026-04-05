import api from './api'
import { TOKEN_KEY, REFRESH_KEY } from '@/utils/constants'
import type { ApiResponse, AuthTokens, LoginFormData, RegisterFormData, User } from '@/types'

export interface LoginResponse {
  user: User
  tokens: AuthTokens
}

const authService = {
  async login(data: LoginFormData): Promise<LoginResponse> {
    const res = await api.post<ApiResponse<LoginResponse>>('/auth/login', data)
    const { user, tokens } = res.data.data
    localStorage.setItem(TOKEN_KEY, tokens.accessToken)
    localStorage.setItem(REFRESH_KEY, tokens.refreshToken)
    return { user, tokens }
  },

  async register(data: RegisterFormData): Promise<LoginResponse> {
    const res = await api.post<ApiResponse<LoginResponse>>('/auth/register', data)
    const { user, tokens } = res.data.data
    localStorage.setItem(TOKEN_KEY, tokens.accessToken)
    localStorage.setItem(REFRESH_KEY, tokens.refreshToken)
    return { user, tokens }
  },

  async logout(): Promise<void> {
    try {
      const refreshToken = localStorage.getItem(REFRESH_KEY)
      await api.post('/auth/logout', { refreshToken })
    } finally {
      localStorage.removeItem(TOKEN_KEY)
      localStorage.removeItem(REFRESH_KEY)
    }
  },

  async getMe(): Promise<User> {
    const res = await api.get<ApiResponse<User>>('/auth/me')
    return res.data.data
  },

  async refreshToken(): Promise<AuthTokens> {
    const refreshToken = localStorage.getItem(REFRESH_KEY)
    const res = await api.post<ApiResponse<AuthTokens>>('/auth/refresh', { refreshToken })
    const tokens = res.data.data
    localStorage.setItem(TOKEN_KEY, tokens.accessToken)
    localStorage.setItem(REFRESH_KEY, tokens.refreshToken)
    return tokens
  },

  isAuthenticated(): boolean {
    return !!localStorage.getItem(TOKEN_KEY)
  },
}

export default authService
