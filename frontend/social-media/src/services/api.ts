import axios, { AxiosError, type InternalAxiosRequestConfig } from 'axios'
import { API_BASE_URL, TOKEN_KEY, REFRESH_KEY } from '@/utils/constants'
import type { AuthTokens } from '@/types'

// ─── Axios Instance ───────────────────────────────────────────────────────────
const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
  timeout: 15_000,
})

// ─── Request Interceptor — inject access token ───────────────────────────────
api.interceptors.request.use((config: InternalAxiosRequestConfig) => {
  const token = localStorage.getItem(TOKEN_KEY)
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// ─── Response Interceptor — refresh token on 401 ─────────────────────────────
let isRefreshing = false
let failedQueue: Array<{ resolve: (v: string) => void; reject: (e: unknown) => void }> = []

function processQueue(error: unknown, token: string | null) {
  failedQueue.forEach(p => (error ? p.reject(error) : p.resolve(token!)))
  failedQueue = []
}

api.interceptors.response.use(
  res => res,
  async (error: AxiosError) => {
    const original = error.config as InternalAxiosRequestConfig & { _retry?: boolean }
    if (error.response?.status !== 401 || original._retry) {
      return Promise.reject(error)
    }

    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        failedQueue.push({ resolve, reject })
      }).then(token => {
        original.headers!.Authorization = `Bearer ${token}`
        return api(original)
      })
    }

    original._retry = true
    isRefreshing = true

    try {
      const refreshToken = localStorage.getItem(REFRESH_KEY)
      if (!refreshToken) throw new Error('No refresh token')

      const { data } = await axios.post<AuthTokens>(`${API_BASE_URL}/auth/refresh`, {
        refreshToken,
      })

      localStorage.setItem(TOKEN_KEY, data.accessToken)
      localStorage.setItem(REFRESH_KEY, data.refreshToken)

      api.defaults.headers.common.Authorization = `Bearer ${data.accessToken}`
      processQueue(null, data.accessToken)

      return api(original)
    } catch (err) {
      processQueue(err, null)
      localStorage.removeItem(TOKEN_KEY)
      localStorage.removeItem(REFRESH_KEY)
      window.location.href = '/login'
      return Promise.reject(err)
    } finally {
      isRefreshing = false
    }
  }
)

export default api
