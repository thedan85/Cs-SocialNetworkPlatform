export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  errors: string[];
}

export interface AuthUser {
  userId: string;
  userName: string;
  email: string;
  bio?: string | null;
}

export interface TokenResponse {
  accessToken: string;
  expiresAt: string;
  tokenType: string;
  roles: string[];
}

export interface AuthTokenResponse {
  user: AuthUser;
  token: TokenResponse;
}

export interface User {
  userId: string;
  userName: string;
  email: string;
  profilePicture?: string | null;
  bio?: string | null;
  createdAt?: string;
  updatedAt?: string;
  isActive?: boolean;
}

export interface Post {
  postId: string;
  userId: string;
  content: string;
  imageUrl?: string | null;
  likeCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface Comment {
  commentId: string;
  postId: string;
  userId: string;
  content: string;
  likeCount: number;
  createdAt: string;
  updatedAt: string;
}

export interface Like {
  likeId: string;
  userId: string;
  postId?: string | null;
  createdAt: string;
}

export interface PostReport {
  postReportId: string;
  postId: string;
  reporterUserId: string;
  reason?: string | null;
  description?: string | null;
  status: boolean;
  createdAt: string;
}

export interface Story {
  storyId: string;
  userId: string;
  content: string;
  imageUrl?: string | null;
  createdAt: string;
  expiresAt: string;
}

export interface Friendship {
  friendshipId: string;
  userId1: string;
  userId2: string;
  status: string;
  createdAt: string;
  updatedAt?: string | null;
}

export interface Notification {
  notificationId: string;
  recipientUserId: string;
  senderUserId: string;
  type?: string | null;
  content?: string | null;
  createdAt: string;
  isRead: boolean;
}
