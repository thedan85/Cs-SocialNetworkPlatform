import { useEffect, useMemo, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { getUserById, getUserPosts } from '../services/users';
import { resolveImageUrl } from '../utils/resolveImageUrl';
import type { Post, User } from '../types';
import PostCard from '../components/specific/PostCard';
import PostSkeleton from '../components/common/PostSkeleton';

const UserProfile = () => {
  const { userId } = useParams();
  const { user } = useAuth();
  const [profile, setProfile] = useState<User | null>(null);
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

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
