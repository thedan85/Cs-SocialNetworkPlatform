export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  errors: string[];
}

export interface AuthUser {
  userId: string;
  userName: string;
  email: string;
  firstName?: string | null;
  lastName?: string | null;
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
  firstName?: string | null;
  lastName?: string | null;
  profilePicture?: string | null;
  bio?: string | null;
  createdAt?: string;
  updatedAt?: string;
  isActive?: boolean;
}

export interface Post {
  postId: string;
  userId: string;
  userName?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  content: string;
  imageUrl?: string | null;
  privacy?: 'Public' | 'Friends' | 'Private' | string;
  sharedPostId?: string | null;
  sharedPost?: SharedPostPreview | null;
  likeCount: number;
  isLiked?: boolean;
  shareCount?: number;
  isShared?: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface SharedPostPreview {
  postId: string;
  userId: string;
  userName?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  content: string;
  imageUrl?: string | null;
  privacy?: 'Public' | 'Friends' | 'Private' | string;
  createdAt: string;
  updatedAt: string;
}

export interface HashtagSearchResult {
  hashtagId: string;
  tag: string;
  usageCount: number;
  posts: Post[];
}

export interface HashtagTrendingResult {
  hashtagId: string;
  tag: string;
  usageCount: number;
}

export interface Comment {
  commentId: string;
  postId: string;
  userId: string;
  userName?: string | null;
  firstName?: string | null;
  lastName?: string | null;
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

export interface PostShare {
  postShareId: string;
  postId: string;
  userId: string;
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

export interface PostReportDetail {
  postReportId: string;
  postId: string;
  reporterUserId: string;
  reporterUserName?: string | null;
  reporterFirstName?: string | null;
  reporterLastName?: string | null;
  reason?: string | null;
  description?: string | null;
  status: boolean;
  createdAt: string;
  postAuthorUserId: string;
  postAuthorUserName?: string | null;
  postAuthorFirstName?: string | null;
  postAuthorLastName?: string | null;
  postContent: string;
  postImageUrl?: string | null;
  postPrivacy?: string | null;
  postCreatedAt: string;
}

export interface Story {
  storyId: string;
  userId: string;
  userName?: string | null;
  firstName?: string | null;
  lastName?: string | null;
  content: string;
  imageUrl?: string | null;
  createdAt: string;
  expiresAt: string;
}

export interface Friendship {
  friendshipId: string;
  userId1: string;
  userId2: string;
  user1Name?: string | null;
  user1FirstName?: string | null;
  user1LastName?: string | null;
  user2Name?: string | null;
  user2FirstName?: string | null;
  user2LastName?: string | null;
  status: string;
  createdAt: string;
  updatedAt?: string | null;
}

export interface FriendRelationship {
  status: string;
  friendshipId?: string | null;
  requesterUserId?: string | null;
  addresseeUserId?: string | null;
  isRequester: boolean;
  isAddressee: boolean;
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
