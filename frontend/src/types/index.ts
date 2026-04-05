export interface User {
  id: string;
  username: string;
  email: string;
  avatarUrl?: string;
}

export interface Post {
  id: string;
  content: string;
  imageUrl?: string;
  createdAt: string;
  author: User;
  likesCount: number;
}
