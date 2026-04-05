export const API_BASE_URL = import.meta.env.VITE_API_URL ?? '/api'
export const SIGNALR_HUB_URL = import.meta.env.VITE_HUB_URL ?? '/hubs/notifications'

export const TOKEN_KEY   = 'sm_access_token'
export const REFRESH_KEY = 'sm_refresh_token'

export const ROUTES = {
  HOME:         '/',
  LOGIN:        '/login',
  REGISTER:     '/register',
  PROFILE:      '/profile/:username',
  SEARCH:       '/search',
  NOTIFICATIONS:'/notifications',
  SETTINGS:     '/settings',
  NOT_FOUND:    '*',
} as const

export const PAGE_SIZE = 10

export const PASSWORD_RULES = {
  MIN_LENGTH:       8,
  REQUIRE_UPPER:    /[A-Z]/,
  REQUIRE_LOWER:    /[a-z]/,
  REQUIRE_NUMBER:   /[0-9]/,
  REQUIRE_SPECIAL:  /[!@#$%^&*(),.?":{}|<>]/,
} as const

export const FILE_UPLOAD = {
  MAX_SIZE_MB:     10,
  ALLOWED_TYPES:   ['image/jpeg', 'image/png', 'image/webp', 'image/gif'],
  ALLOWED_EXTS:    '.jpg,.jpeg,.png,.webp,.gif',
} as const
