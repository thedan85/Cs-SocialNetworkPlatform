import { useEffect, useMemo, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import {
  acceptFriendRequest,
  createFriendRequest,
  getFriendRelationship,
  rejectFriendRequest,
  removeFriend
} from '../services/friends';
import { getUserById, getUserPosts } from '../services/users';
import { resolveImageUrl } from '../utils/resolveImageUrl';
import type { FriendRelationship, Post, User } from '../types';
import PostCard from '../components/specific/PostCard';
import PostSkeleton from '../components/common/PostSkeleton';

const UserProfile = () => {
  const { userId } = useParams();
  const { user } = useAuth();
  const [profile, setProfile] = useState<User | null>(null);
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [relationship, setRelationship] = useState<FriendRelationship | null>(null);
  const [relationshipLoading, setRelationshipLoading] = useState(false);
  const [relationshipError, setRelationshipError] = useState<string | null>(null);
  const [relationshipActionLoading, setRelationshipActionLoading] = useState(false);

  const isCurrentUser = useMemo(() => {
    if (!userId || !user) return false;
    return user.userId === userId;
  }, [user, userId]);

  useEffect(() => {
    const loadProfile = async () => {
      if (!userId) {
        setError('User not found.');
        setLoading(false);
        return;
      }

      setLoading(true);
      setError(null);
      try {
        const [profileData, postsData] = await Promise.all([
          getUserById(userId),
          getUserPosts(userId)
        ]);
        setProfile(profileData);
        setPosts(postsData);
      } catch (err: any) {
        setError(err?.message || 'Unable to load profile.');
      } finally {
        setLoading(false);
      }
    };

    loadProfile();
  }, [userId]);

  useEffect(() => {
    if (!userId || !user || isCurrentUser) {
      setRelationship(null);
      setRelationshipError(null);
      return;
    }

    let isActive = true;

    const loadRelationship = async () => {
      setRelationshipLoading(true);
      setRelationshipError(null);
      try {
        const data = await getFriendRelationship(userId);
        if (isActive) {
          setRelationship(data);
        }
      } catch (err: any) {
        if (isActive) {
          setRelationshipError(err?.message || 'Unable to load relationship.');
        }
      } finally {
        if (isActive) {
          setRelationshipLoading(false);
        }
      }
    };

    loadRelationship();

    return () => {
      isActive = false;
    };
  }, [userId, user, isCurrentUser]);

  if (loading) {
    return (
      <div className="max-w-3xl mx-auto space-y-6">
        <div className="rounded-2xl border border-white/60 bg-white/70 p-6 shadow-[0_12px_30px_rgba(15,23,42,0.08)] dark:border-slate-800/60 dark:bg-slate-900/60">
          <div className="h-6 w-40 animate-pulse rounded-full bg-slate-200/60 dark:bg-slate-700/60" />
          <div className="mt-2 h-4 w-56 animate-pulse rounded-full bg-slate-200/60 dark:bg-slate-700/60" />
        </div>
        <PostSkeleton />
        <PostSkeleton />
      </div>
    );
  }

  if (error) {
    return (
      <div className="max-w-3xl mx-auto">
        <div className="rounded-xl border border-rose-200/70 bg-rose-50/70 px-4 py-3 text-sm text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300">
          {error}
        </div>
      </div>
    );
  }

  if (!profile) {
    return (
      <div className="max-w-3xl mx-auto text-slate-600 dark:text-slate-400">
        User not found.
      </div>
    );
  }

  const displayName = [profile.firstName, profile.lastName].filter(Boolean).join(' ').trim() || profile.userName;
  const avatar = resolveImageUrl(profile.profilePicture)
    || `https://ui-avatars.com/api/?name=${encodeURIComponent(displayName)}`;
  const relationshipStatus = (relationship?.status || 'None').toLowerCase();
  const isFriend = relationshipStatus === 'accepted';
  const isPending = relationshipStatus === 'pending';
  const isRejected = relationshipStatus === 'rejected';
  const canSendRequest = relationshipStatus === 'none';

  const refreshRelationship = async () => {
    if (!userId || !user || isCurrentUser) return;
    const data = await getFriendRelationship(userId);
    setRelationship(data);
  };

  const handleSendRequest = async () => {
    if (!userId) return;
    setRelationshipActionLoading(true);
    try {
      await createFriendRequest({ addresseeUserId: userId });
      await refreshRelationship();
    } catch (err) {
      alert('Unable to send friend request.');
    } finally {
      setRelationshipActionLoading(false);
    }
  };

  const handleAccept = async () => {
    if (!relationship?.friendshipId) return;
    setRelationshipActionLoading(true);
    try {
      await acceptFriendRequest(relationship.friendshipId);
      await refreshRelationship();
    } catch (err) {
      alert('Unable to accept request.');
    } finally {
      setRelationshipActionLoading(false);
    }
  };

  const handleReject = async () => {
    if (!relationship?.friendshipId) return;
    setRelationshipActionLoading(true);
    try {
      await rejectFriendRequest(relationship.friendshipId);
      await refreshRelationship();
    } catch (err) {
      alert('Unable to reject request.');
    } finally {
      setRelationshipActionLoading(false);
    }
  };

  const handleUnfriend = async () => {
    if (!relationship?.friendshipId) return;
    setRelationshipActionLoading(true);
    try {
      await removeFriend(relationship.friendshipId);
      await refreshRelationship();
    } catch (err) {
      alert('Unable to remove friend.');
    } finally {
      setRelationshipActionLoading(false);
    }
  };

  return (
    <div className="max-w-3xl mx-auto space-y-6">
      <div className="rounded-2xl border border-white/60 bg-white/80 p-6 shadow-[0_16px_40px_rgba(15,23,42,0.1)] dark:border-slate-800/60 dark:bg-slate-900/60">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div className="flex items-center gap-4">
            <div className="h-20 w-20 overflow-hidden rounded-full border border-white/70 bg-white/70 shadow-md dark:border-slate-700/60 dark:bg-slate-900/60">
              <img src={avatar} alt={displayName} className="h-full w-full object-cover" />
            </div>
            <div>
              <h1 className="text-2xl font-bold text-slate-900 dark:text-slate-100">{displayName}</h1>
              <p className="text-sm text-slate-500 dark:text-slate-400">@{profile.userName}</p>
              {profile.bio && (
                <p className="mt-2 text-sm text-slate-600 dark:text-slate-300">{profile.bio}</p>
              )}
            </div>
          </div>
          {isCurrentUser && (
            <Link
              to="/profile"
              className="rounded-xl bg-gradient-to-r from-teal-500 via-cyan-500 to-amber-400 px-4 py-2 text-sm font-semibold text-white shadow-lg shadow-cyan-500/25 hover:from-teal-600 hover:via-cyan-600 hover:to-amber-500"
            >
              Edit profile
            </Link>
          )}
          {!isCurrentUser && user && (
            <div className="flex flex-wrap items-center gap-2">
              {relationshipLoading && (
                <span className="text-xs text-slate-400 dark:text-slate-500">Checking relationship...</span>
              )}
              {relationshipError && (
                <span className="text-xs text-rose-500 dark:text-rose-300">{relationshipError}</span>
              )}
              {!relationshipLoading && !relationshipError && (
                <>
                  {isFriend && (
                    <>
                      <span className="rounded-full bg-cyan-100/70 px-3 py-1 text-xs font-semibold text-cyan-700 dark:bg-cyan-500/10 dark:text-cyan-200">
                        Friends
                      </span>
                      <button
                        type="button"
                        onClick={handleUnfriend}
                        disabled={relationshipActionLoading}
                        className="rounded-xl border border-rose-200/70 bg-rose-50/70 px-4 py-2 text-xs font-semibold text-rose-700 hover:bg-rose-100 disabled:opacity-60 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300"
                      >
                        {relationshipActionLoading ? 'Removing...' : 'Unfriend'}
                      </button>
                    </>
                  )}
                  {isPending && relationship?.isAddressee && (
                    <>
                      <button
                        type="button"
                        onClick={handleAccept}
                        disabled={relationshipActionLoading}
                        className="rounded-xl bg-emerald-500 px-4 py-2 text-xs font-semibold text-white shadow-lg shadow-emerald-500/20 hover:bg-emerald-600 disabled:opacity-60"
                      >
                        {relationshipActionLoading ? 'Accepting...' : 'Accept'}
                      </button>
                      <button
                        type="button"
                        onClick={handleReject}
                        disabled={relationshipActionLoading}
                        className="rounded-xl border border-slate-200/70 bg-white/80 px-4 py-2 text-xs font-semibold text-slate-600 hover:bg-slate-100 disabled:opacity-60 dark:border-slate-700/60 dark:bg-slate-900/70 dark:text-slate-300"
                      >
                        Reject
                      </button>
                    </>
                  )}
                  {isPending && relationship?.isRequester && (
                    <span className="rounded-full bg-amber-100/70 px-3 py-1 text-xs font-semibold text-amber-700 dark:bg-amber-500/10 dark:text-amber-200">
                      Request sent
                    </span>
                  )}
                  {isRejected && (
                    <span className="rounded-full bg-slate-100/70 px-3 py-1 text-xs font-semibold text-slate-600 dark:bg-slate-500/10 dark:text-slate-300">
                      Request rejected
                    </span>
                  )}
                  {canSendRequest && (
                    <button
                      type="button"
                      onClick={handleSendRequest}
                      disabled={relationshipActionLoading}
                      className="rounded-xl bg-gradient-to-r from-teal-500 via-cyan-500 to-amber-400 px-4 py-2 text-xs font-semibold text-white shadow-lg shadow-cyan-500/25 hover:from-teal-600 hover:via-cyan-600 hover:to-amber-500 disabled:opacity-60"
                    >
                      {relationshipActionLoading ? 'Sending...' : 'Add friend'}
                    </button>
                  )}
                </>
              )}
            </div>
          )}
        </div>
      </div>

      <div className="space-y-4">
        <div className="flex items-center justify-between">
          <h2 className="text-lg font-semibold text-slate-900 dark:text-slate-100">Posts</h2>
          <span className="text-sm text-slate-500 dark:text-slate-400">{posts.length} total</span>
        </div>

        {posts.length === 0 ? (
          <div className="rounded-2xl border border-white/60 bg-white/70 px-4 py-6 text-center text-sm text-slate-500 shadow-[0_12px_30px_rgba(15,23,42,0.08)] dark:border-slate-800/60 dark:bg-slate-900/60 dark:text-slate-400">
            No posts to show yet.
          </div>
        ) : (
          posts.map((post) => <PostCard key={post.postId} post={post} />)
        )}
      </div>
    </div>
  );
};

export default UserProfile;
