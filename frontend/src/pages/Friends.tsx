import { FormEvent, useEffect, useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import type { Friendship } from '../types';
import {
  acceptFriendRequest,
  createFriendRequest,
  getFriends,
  getPendingRequests,
  rejectFriendRequest
} from '../services/friends';

const Friends = () => {
  const { user } = useAuth();
  const [friends, setFriends] = useState<Friendship[]>([]);
  const [pending, setPending] = useState<Friendship[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [addresseeUserId, setAddresseeUserId] = useState('');
  const [sending, setSending] = useState(false);

  const loadFriends = async () => {
    if (!user) return;
    setLoading(true);
    setError(null);
    try {
      const [friendsData, pendingData] = await Promise.all([
        getFriends(user.userId),
        getPendingRequests(user.userId)
      ]);
      setFriends(friendsData);
      setPending(pendingData);
    } catch (err: any) {
      setError(err.message || 'Failed to load friends.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadFriends();
  }, [user]);

  const handleSendRequest = async (event: FormEvent) => {
    event.preventDefault();
    if (!addresseeUserId.trim()) return;
    setSending(true);
    try {
      const request = await createFriendRequest({ addresseeUserId: addresseeUserId.trim() });
      setPending((current) => [request, ...current]);
      setAddresseeUserId('');
    } catch (err) {
      alert('Unable to send friend request.');
    } finally {
      setSending(false);
    }
  };

  const handleAccept = async (friendshipId: string) => {
    try {
      const updated = await acceptFriendRequest(friendshipId);
      setPending((current) => current.filter((item) => item.friendshipId !== friendshipId));
      setFriends((current) => [updated, ...current]);
    } catch (err) {
      alert('Unable to accept request.');
    }
  };

  const handleReject = async (friendshipId: string) => {
    try {
      await rejectFriendRequest(friendshipId);
      setPending((current) => current.filter((item) => item.friendshipId !== friendshipId));
    } catch (err) {
      alert('Unable to reject request.');
    }
  };

  const resolveFriendLabel = (friendship: Friendship) => {
    if (!user) return friendship.userId2;
    return friendship.userId1 === user.userId ? friendship.userId2 : friendship.userId1;
  };

  if (!user) {
    return <div className="text-center text-gray-600">Sign in to manage friends.</div>;
  }

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-gray-800">Friends</h1>
        <p className="text-sm text-gray-500">Manage friend requests and connections.</p>
      </div>

      <form onSubmit={handleSendRequest} className="rounded-xl border border-gray-200 bg-white p-4 shadow-sm">
        <h2 className="text-lg font-semibold text-gray-800">Send Friend Request</h2>
        <div className="mt-3 flex flex-col gap-3 sm:flex-row sm:items-center">
          <input
            value={addresseeUserId}
            onChange={(event) => setAddresseeUserId(event.target.value)}
            placeholder="Enter user id"
            className="flex-1 rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-200"
          />
          <button
            type="submit"
            disabled={sending}
            className="rounded-lg bg-blue-600 px-4 py-2 text-sm font-semibold text-white hover:bg-blue-700 disabled:bg-gray-400"
          >
            {sending ? 'Sending...' : 'Send request'}
          </button>
        </div>
      </form>

      {loading && <div className="text-gray-600">Loading friends...</div>}
      {error && (
        <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {error}
        </div>
      )}

      <div className="grid gap-4 md:grid-cols-2">
        <div className="rounded-xl border border-gray-200 bg-white p-4 shadow-sm">
          <h2 className="text-lg font-semibold text-gray-800">Pending Requests</h2>
          {pending.length === 0 ? (
            <p className="mt-3 text-sm text-gray-500">No pending requests.</p>
          ) : (
            <div className="mt-3 space-y-3">
              {pending.map((item) => (
                <div key={item.friendshipId} className="flex items-center justify-between gap-3">
                  <div>
                    <p className="text-sm font-medium text-gray-800">
                      {resolveFriendLabel(item)}
                    </p>
                    <p className="text-xs text-gray-400">Status: {item.status}</p>
                  </div>
                  <div className="flex gap-2">
                    <button
                      onClick={() => handleAccept(item.friendshipId)}
                      className="rounded-md bg-green-600 px-3 py-1 text-xs font-semibold text-white hover:bg-green-700"
                    >
                      Accept
                    </button>
                    <button
                      onClick={() => handleReject(item.friendshipId)}
                      className="rounded-md bg-gray-200 px-3 py-1 text-xs font-semibold text-gray-700 hover:bg-gray-300"
                    >
                      Reject
                    </button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        <div className="rounded-xl border border-gray-200 bg-white p-4 shadow-sm">
          <h2 className="text-lg font-semibold text-gray-800">Friends</h2>
          {friends.length === 0 ? (
            <p className="mt-3 text-sm text-gray-500">No friends yet.</p>
          ) : (
            <div className="mt-3 space-y-3">
              {friends.map((item) => (
                <div key={item.friendshipId} className="flex items-center justify-between gap-3">
                  <div>
                    <p className="text-sm font-medium text-gray-800">
                      {resolveFriendLabel(item)}
                    </p>
                    <p className="text-xs text-gray-400">Status: {item.status}</p>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Friends;
