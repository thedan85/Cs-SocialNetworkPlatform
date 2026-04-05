// ─── Auth ──────────────────────────────────────────────────────────────────
export interface User {
  id: string
  username: string
  email: string
  fullName: string
  bio?: string
  avatarUrl?: string
  coverUrl?: string
  followersCount: number
  followingCount: number
  postsCount: number
  isFollowing?: boolean
  isFriend?: boolean
  createdAt: string
}

export interface AuthTokens {
  accessToken: string
  refreshToken: string
}

export interface AuthState {
  user: User | null
  tokens: AuthTokens | null
  isAuthenticated: boolean
  isLoading: boolean
}

// ─── Post ───────────────────────────────────────────────────────────────────
export interface Post {
  id: string
  content: string
  imageUrl?: string
  author: User
  likesCount: number
  commentsCount: number
  sharesCount: number
  isLiked: boolean
  isSaved: boolean
  createdAt: string
  updatedAt: string
}

export interface Comment {
  id: string
  content: string
  author: User
  postId: string
  likesCount: number
  isLiked: boolean
  createdAt: string
}

// ─── Notification ────────────────────────────────────────────────────────────
export type NotificationType =
  | 'like'
  | 'comment'
  | 'follow'
  | 'friend_request'
  | 'mention'
  | 'share'

export interface Notification {
  id: string
  type: NotificationType
  message: string
  isRead: boolean
  sender: Pick<User, 'id' | 'username' | 'avatarUrl'>
  postId?: string
  createdAt: string
}

// ─── Friend Request ──────────────────────────────────────────────────────────
export type FriendStatus = 'none' | 'pending' | 'friends' | 'blocked'

export interface FriendRequest {
  id: string
  sender: User
  receiver: User
  status: 'pending' | 'accepted' | 'rejected'
  createdAt: string
}

// ─── API Responses ───────────────────────────────────────────────────────────
export interface ApiResponse<T> {
  data: T
  message: string
  success: boolean
}

export interface PaginatedResponse<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export interface ApiError {
  message: string
  errors?: Record<string, string[]>
  statusCode: number
}

// ─── Form Types ───────────────────────────────────────────────────────────────
export interface LoginFormData {
  email: string
  password: string
  rememberMe?: boolean
}

export interface RegisterFormData {
  username: string
  email: string
  password: string
  confirmPassword: string
  fullName: string
}

export interface PostFormData {
  content: string
  image?: FileList
}

export interface ProfileUpdateFormData {
  fullName: string
  bio?: string
  avatar?: FileList
  cover?: FileList
}

// ─── Search ───────────────────────────────────────────────────────────────────
export interface SearchResult {
  users: User[]
  posts: Post[]
}

export interface SearchParams {
  query: string
  type?: 'all' | 'users' | 'posts'
  page?: number
  pageSize?: number
}
