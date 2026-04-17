import { FormEvent, useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import type { Friendship, User } from '../types';
import {
  acceptFriendRequest,
  createFriendRequest,
  getFriends,
  getPendingRequests,
  rejectFriendRequest
} from '../services/friends';
import { searchUsers } from '../services/users';

const Friends = () => {
  const { user } = useAuth();
  const [friends, setFriends] = useState<Friendship[]>([]);
  const [pending, setPending] = useState<Friendship[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [sending, setSending] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [searchResults, setSearchResults] = useState<User[]>([]);
  const [searching, setSearching] = useState(false);
  const [searchError, setSearchError] = useState<string | null>(null);

  const buildDisplayName = (
    firstName?: string | null,
    lastName?: string | null,
    userName?: string | null,
    fallbackId?: string
  ) => {
    const fullName = [firstName, lastName].filter(Boolean).join(' ').trim();
    if (fullName) return fullName;
    if (userName) return userName;
    if (fallbackId) return `User ${fallbackId.slice(0, 8)}`;
    return 'Unknown user';
  };

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

  const handleSearch = async (event: FormEvent) => {
    event.preventDefault();
    const query = searchQuery.trim();
    if (!query) return;

    setSearching(true);
    setSearchError(null);
    try {
      const results = await searchUsers(query);
      const filtered = results.filter(result => result.userId !== user?.userId);
      setSearchResults(filtered);
    } catch (err: any) {
      setSearchError(err.message || 'Unable to search users.');
    } finally {
      setSearching(false);
    }
  };

  const handleSendRequest = async (addresseeUserId: string) => {
    if (!addresseeUserId) return;
    setSending(true);
    try {
      const request = await createFriendRequest({ addresseeUserId });
      setPending((current) => [request, ...current]);
      setSearchResults((current) => current.filter(result => result.userId !== addresseeUserId));
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

  const resolveFriendInfo = (friendship: Friendship) => {
    if (!user) {
      return {
        label: buildDisplayName(
        friendship.user2FirstName,
        friendship.user2LastName,
        friendship.user2Name,
        friendship.userId2
        ),
        userId: friendship.userId2 || friendship.userId1
      };
    }

    const isUser1 = friendship.userId1 === user.userId;
    const friendFirstName = isUser1 ? friendship.user2FirstName : friendship.user1FirstName;
    const friendLastName = isUser1 ? friendship.user2LastName : friendship.user1LastName;
    const friendUserName = isUser1 ? friendship.user2Name : friendship.user1Name;
    const friendId = isUser1 ? friendship.userId2 : friendship.userId1;

    return {
      label: buildDisplayName(friendFirstName, friendLastName, friendUserName, friendId),
      userId: friendId
    };
  };

  if (!user) {
    return <div className="text-center text-slate-600 dark:text-slate-400">Sign in to manage friends.</div>;
  }

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <div>
        <h1 className="text-2xl font-bold text-slate-900 tracking-tight dark:text-slate-100">Friends</h1>
        <p className="text-sm text-slate-500 dark:text-slate-400">Manage friend requests and connections.</p>
      </div>

      <form onSubmit={handleSearch} className="rounded-2xl border border-white/60 bg-white/70 backdrop-blur-xl p-4 shadow-[0_12px_30px_rgba(15,23,42,0.08)] dark:bg-slate-900/60 dark:border-slate-800/60">
        <h2 className="text-lg font-semibold text-slate-900 dark:text-slate-100">Find People</h2>
        <p className="mt-1 text-sm text-slate-500 dark:text-slate-400">Search by first name, last name, or username.</p>
        <div className="mt-3 flex flex-col gap-3 sm:flex-row sm:items-center">
          <input
            value={searchQuery}
            onChange={(event) => setSearchQuery(event.target.value)}
            placeholder="Search by name or username"
            className="flex-1 rounded-xl border border-white/70 bg-white/80 px-3 py-2 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-300/70 dark:bg-slate-900/70 dark:border-slate-700/60 dark:text-slate-100 dark:placeholder:text-slate-500 dark:focus:ring-cyan-500/40"
          />
          <button
            type="submit"
            disabled={searching}
            className="rounded-xl bg-gradient-to-r from-teal-500 via-cyan-500 to-amber-400 px-4 py-2 text-sm font-semibold text-white shadow-lg shadow-cyan-500/25 hover:from-teal-600 hover:via-cyan-600 hover:to-amber-500 disabled:from-slate-300 disabled:via-slate-300 disabled:to-slate-300 disabled:text-slate-500 disabled:shadow-none"
          >
            {searching ? 'Searching...' : 'Search'}
          </button>
        </div>

        {searchError && (
          <div className="mt-3 rounded-xl border border-rose-200/70 bg-rose-50/70 px-3 py-2 text-sm text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300">
            {searchError}
          </div>
        )}

        {!searching && searchResults.length > 0 && (
          <div className="mt-4 space-y-3">
            {searchResults.map((result) => (
              <div key={result.userId} className="flex items-center justify-between rounded-xl border border-white/60 bg-white/70 px-3 py-2 dark:border-slate-800/60 dark:bg-slate-900/60">
                <div>
                  <Link
                    to={`/users/${result.userId}`}
                    className="text-sm font-semibold text-slate-900 hover:text-cyan-700 dark:text-slate-100 dark:hover:text-cyan-200"
                  >
                    {buildDisplayName(result.firstName, result.lastName, result.userName, result.userId)}
                  </Link>
                  <p className="text-xs text-slate-400 dark:text-slate-500">@{result.userName}</p>
                </div>
                <button
                  type="button"
                  onClick={() => handleSendRequest(result.userId)}
                  disabled={sending}
                  className="rounded-lg bg-emerald-500 px-3 py-1 text-xs font-semibold text-white shadow-md shadow-emerald-500/20 hover:bg-emerald-600 disabled:bg-emerald-300"
                >
                  {sending ? 'Sending...' : 'Add'}
                </button>
              </div>
            ))}
          </div>
        )}

        {!searching && searchQuery.trim().length > 0 && searchResults.length === 0 && !searchError && (
          <div className="mt-3 text-sm text-slate-500 dark:text-slate-400">No users found.</div>
        )}
      </form>

      {loading && <div className="text-slate-600 dark:text-slate-400">Loading friends...</div>}
      {error && (
        <div className="rounded-xl border border-rose-200/70 bg-rose-50/70 px-4 py-3 text-sm text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300">
          {error}
        </div>
      )}

      <div className="grid gap-4 md:grid-cols-2">
        <div className="rounded-2xl border border-white/60 bg-white/70 backdrop-blur-xl p-4 shadow-[0_12px_30px_rgba(15,23,42,0.08)] dark:bg-slate-900/60 dark:border-slate-800/60">
          <h2 className="text-lg font-semibold text-slate-900 dark:text-slate-100">Pending Requests</h2>
          {pending.length === 0 ? (
            <p className="mt-3 text-sm text-slate-500 dark:text-slate-400">No pending requests.</p>
          ) : (
            <div className="mt-3 space-y-3">
              {pending.map((item) => {
                const friendInfo = resolveFriendInfo(item);
                return (
                  <div key={item.friendshipId} className="flex items-center justify-between gap-3">
                    <div>
                      <Link
                        to={`/users/${friendInfo.userId}`}
                        className="text-sm font-medium text-slate-900 hover:text-cyan-700 dark:text-slate-100 dark:hover:text-cyan-200"
                      >
                        {friendInfo.label}
                      </Link>
                    <p className="text-xs text-slate-400 dark:text-slate-500">Status: {item.status}</p>
                    </div>
                    <div className="flex gap-2">
                      <button
                        onClick={() => handleAccept(item.friendshipId)}
                        className="rounded-md bg-emerald-500 px-3 py-1 text-xs font-semibold text-white shadow-md shadow-emerald-500/20 hover:bg-emerald-600"
                      >
                        Accept
                      </button>
                      <button
                        onClick={() => handleReject(item.friendshipId)}
                        className="rounded-md bg-white/80 px-3 py-1 text-xs font-semibold text-slate-600 border border-white/70 hover:bg-white dark:bg-slate-900/60 dark:text-slate-200 dark:border-slate-700/60 dark:hover:bg-slate-800/70"
                      >
                        Reject
                      </button>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>

        <div className="rounded-2xl border border-white/60 bg-white/70 backdrop-blur-xl p-4 shadow-[0_12px_30px_rgba(15,23,42,0.08)] dark:bg-slate-900/60 dark:border-slate-800/60">
          <h2 className="text-lg font-semibold text-slate-900 dark:text-slate-100">Friends</h2>
          {friends.length === 0 ? (
            <p className="mt-3 text-sm text-slate-500 dark:text-slate-400">No friends yet.</p>
          ) : (
            <div className="mt-3 space-y-3">
              {friends.map((item) => {
                const friendInfo = resolveFriendInfo(item);
                return (
                  <div key={item.friendshipId} className="flex items-center justify-between gap-3">
                    <div>
                      <Link
                        to={`/users/${friendInfo.userId}`}
                        className="text-sm font-medium text-slate-900 hover:text-cyan-700 dark:text-slate-100 dark:hover:text-cyan-200"
                      >
                        {friendInfo.label}
                      </Link>
                      <p className="text-xs text-slate-400 dark:text-slate-500">Status: {item.status}</p>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Friends;
