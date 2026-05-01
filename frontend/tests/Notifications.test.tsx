import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import Notifications from '../src/pages/Notifications';
import { useAuth } from '../src/contexts/AuthContext';
import {
  deleteNotification,
  getNotifications,
  markNotificationRead
} from '../src/services/notifications';
import { createNotificationHubConnection } from '../src/services/notificationHub';
import type { Notification } from '../src/types';

vi.mock('../src/contexts/AuthContext', () => ({
  useAuth: vi.fn()
}));

vi.mock('../src/services/notifications', () => ({
  getNotifications: vi.fn(),
  markNotificationRead: vi.fn(),
  deleteNotification: vi.fn()
}));

vi.mock('../src/services/notificationHub', () => ({
  createNotificationHubConnection: vi.fn()
}));

const useAuthMock = vi.mocked(useAuth);
const getNotificationsMock = vi.mocked(getNotifications);
const markNotificationReadMock = vi.mocked(markNotificationRead);
const createNotificationHubConnectionMock = vi.mocked(createNotificationHubConnection);

const baseAuthValue = {
  user: {
    userId: 'user-1',
    userName: 'user-1',
    email: 'user-1@example.com'
  },
  roles: [],
  isAdmin: false,
  isAuthenticated: true,
  login: vi.fn(),
  logout: vi.fn(),
  updateUser: vi.fn(),
  loading: false
};

describe('Notifications', () => {
  beforeEach(() => {
    localStorage.clear();
    createNotificationHubConnectionMock.mockReturnValue({
      on: vi.fn(),
      start: vi.fn().mockResolvedValue(undefined),
      stop: vi.fn().mockResolvedValue(undefined)
    } as ReturnType<typeof createNotificationHubConnection>);
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  it('shows sign-in message when no user', () => {
    useAuthMock.mockReturnValue({
      ...baseAuthValue,
      user: null,
      isAuthenticated: false
    });

    render(<Notifications />);

    expect(screen.getByText('Sign in to view notifications.')).toBeTruthy();
  });

  it('shows empty state when no notifications exist', async () => {
    useAuthMock.mockReturnValue(baseAuthValue);
    getNotificationsMock.mockResolvedValue([]);

    render(<Notifications />);

    expect(await screen.findByText('No notifications yet.')).toBeTruthy();
  });

  it('marks notifications as read', async () => {
    useAuthMock.mockReturnValue(baseAuthValue);
    localStorage.setItem('token', 'test-token');

    const notification: Notification = {
      notificationId: 'n1',
      recipientUserId: 'user-1',
      senderUserId: 'user-2',
      type: 'Like',
      content: 'User liked your post',
      createdAt: new Date('2026-05-01T00:00:00Z').toISOString(),
      isRead: false
    };

    getNotificationsMock.mockResolvedValue([notification]);
    markNotificationReadMock.mockResolvedValue({ ...notification, isRead: true });

    render(<Notifications />);

    const markReadButton = await screen.findByRole('button', { name: 'Mark read' });
    await userEvent.click(markReadButton);

    await waitFor(() =>
      expect(markNotificationReadMock).toHaveBeenCalledWith('n1')
    );

    await waitFor(() =>
      expect(screen.queryByRole('button', { name: 'Mark read' })).toBeNull()
    );
  });
});
