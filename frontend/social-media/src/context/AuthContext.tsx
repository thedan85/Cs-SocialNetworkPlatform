import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import authService from '@/services/authService'
import type { AuthState, LoginFormData, RegisterFormData, User } from '@/types'

// ─── Context Types ────────────────────────────────────────────────────────────
interface AuthContextValue extends AuthState {
  login:    (data: LoginFormData) => Promise<void>
  register: (data: RegisterFormData) => Promise<void>
  logout:   () => Promise<void>
  updateUser: (user: User) => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

// ─── Provider ─────────────────────────────────────────────────────────────────
export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>({
    user:            null,
    tokens:          null,
    isAuthenticated: false,
    isLoading:       true,
  })

  // Try to restore session on mount
  useEffect(() => {
    const restore = async () => {
      if (!authService.isAuthenticated()) {
        setState(s => ({ ...s, isLoading: false }))
        return
      }
      try {
        const user = await authService.getMe()
        setState({ user, tokens: null, isAuthenticated: true, isLoading: false })
      } catch {
        setState({ user: null, tokens: null, isAuthenticated: false, isLoading: false })
      }
    }
    restore()
  }, [])

  const login = useCallback(async (data: LoginFormData) => {
    const { user, tokens } = await authService.login(data)
    setState({ user, tokens, isAuthenticated: true, isLoading: false })
  }, [])

  const register = useCallback(async (data: RegisterFormData) => {
    const { user, tokens } = await authService.register(data)
    setState({ user, tokens, isAuthenticated: true, isLoading: false })
  }, [])

  const logout = useCallback(async () => {
    await authService.logout()
    setState({ user: null, tokens: null, isAuthenticated: false, isLoading: false })
  }, [])

  const updateUser = useCallback((user: User) => {
    setState(s => ({ ...s, user }))
  }, [])

  const value = useMemo<AuthContextValue>(
    () => ({ ...state, login, register, logout, updateUser }),
    [state, login, register, logout, updateUser]
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

// ─── Hook ─────────────────────────────────────────────────────────────────────
export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
