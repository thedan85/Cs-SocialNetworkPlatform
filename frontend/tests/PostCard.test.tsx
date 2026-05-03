import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, describe, expect, it, vi } from 'vitest';
import { MemoryRouter } from 'react-router-dom';
import PostCard from '../src/components/specific/PostCard';
import { useAuth } from '../src/contexts/AuthContext';
import {
  getPostComments,
  sharePost,
} from '../src/services/posts';
import type { Post } from '../src/types';

vi.mock('../src/contexts/AuthContext', () => ({
  useAuth: vi.fn()
}));

vi.mock('../src/services/posts', () => ({
  getPostComments: vi.fn(),
  sharePost: vi.fn(),
  updatePost: vi.fn()
}));

const useAuthMock = vi.mocked(useAuth);
const sharePostMock = vi.mocked(sharePost);
const getPostCommentsMock = vi.mocked(getPostComments);

const basePost: Post = {
  postId: 'post-1',
  userId: 'user-2',
  userName: 'creator',
  firstName: 'Post',
  lastName: 'Author',
  profilePicture: null,
  content: 'Original post content',
  imageUrl: null,
  privacy: 'Public',
  sharedPostId: null,
  sharedPost: null,
  likeCount: 0,
  isLiked: false,
  createdAt: '2026-05-03T00:00:00.000Z',
  updatedAt: '2026-05-03T00:00:00.000Z'
};

describe('PostCard', () => {
  afterEach(() => {
    vi.clearAllMocks();
  });

  it('keeps share one-way and only opens the share form', async () => {
    useAuthMock.mockReturnValue({
      user: {
        userId: 'user-1',
        userName: 'viewer',
        email: 'viewer@example.com'
      },
      roles: [],
      isAdmin: false,
      isAuthenticated: true,
      login: vi.fn(),
      logout: vi.fn(),
      updateUser: vi.fn(),
      loading: false
    });

    getPostCommentsMock.mockResolvedValue([]);
    sharePostMock.mockResolvedValue({
      postShareId: 'share-1',
      postId: basePost.postId,
      userId: 'user-1',
      createdAt: new Date().toISOString()
    });

    render(
      <MemoryRouter>
        <PostCard post={basePost} />
      </MemoryRouter>
    );

    expect(screen.getByRole('button', { name: 'Share' })).toBeTruthy();

    await userEvent.click(screen.getByRole('button', { name: 'Share' }));
    await userEvent.type(screen.getByPlaceholderText(/add your thoughts/i), 'Nice post');
    await userEvent.click(screen.getByRole('button', { name: 'Share post' }));

    await waitFor(() =>
      expect(sharePostMock).toHaveBeenCalledWith(basePost.postId, {
        content: 'Nice post',
        privacy: 'Public'
      })
    );
    expect(screen.getByRole('button', { name: 'Share' })).toBeTruthy();
  });
});