import { ChangeEvent, useEffect, useId, useRef, useState } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { useForm } from 'react-hook-form';
import Input from '../components/common/Input';
import { User, LogOut } from 'lucide-react';
import { getUserPosts, updateUser as updateUserRequest } from '../services/users';
import { getFriends, getPendingRequests } from '../services/friends';
import { uploadImage } from '../services/uploads';
import { resolveImageUrl } from '../utils/resolveImageUrl';
import type { Post } from '../types';
import PostCard from '../components/specific/PostCard';
import PostSkeleton from '../components/common/PostSkeleton';

interface UpdateProfileForm {
  firstName?: string;
  lastName?: string;
  bio?: string;
}

const Profile = () => {
  const { user, logout, updateUser } = useAuth();
  const { register, handleSubmit, formState: { isSubmitting } } = useForm<UpdateProfileForm>(
    {
      defaultValues: {
        firstName: user?.firstName || '',
        lastName: user?.lastName || '',
        bio: user?.bio || '',
      },
    }
  );
  const inputId = useId();
  const fileInputRef = useRef<HTMLInputElement | null>(null);
  const [preview, setPreview] = useState<string | null>(user?.profilePicture || null);
  const [previewIsLocal, setPreviewIsLocal] = useState(false);
  const [profileFile, setProfileFile] = useState<File | null>(null);
  const [removeProfilePicture, setRemoveProfilePicture] = useState(false);
  const [uploadingImage, setUploadingImage] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [postsCount, setPostsCount] = useState<number | null>(null);
  const [friendsCount, setFriendsCount] = useState<number | null>(null);
  const [pendingCount, setPendingCount] = useState<number | null>(null);
  const [posts, setPosts] = useState<Post[]>([]);
  const [statsLoading, setStatsLoading] = useState(false);
  const [statsError, setStatsError] = useState<string | null>(null);
  const [postsError, setPostsError] = useState<string | null>(null);
  const previewSrc = preview ? resolveImageUrl(preview) : null;

  useEffect(() => {
    return () => {
      if (previewIsLocal && preview) {
        URL.revokeObjectURL(preview);
      }
    };
  }, [preview, previewIsLocal]);

  useEffect(() => {
    const loadStats = async () => {
      if (!user) {
        setPostsCount(null);
        setFriendsCount(null);
        setPendingCount(null);
        return;
      }

      setStatsLoading(true);
      setStatsError(null);
      setPostsError(null);
      try {
        const [posts, friends, pending] = await Promise.all([
          getUserPosts(user.userId),
          getFriends(user.userId),
          getPendingRequests(user.userId)
        ]);
        setPosts(posts);
        setPostsCount(posts.length);
        setFriendsCount(friends.length);
        setPendingCount(pending.length);
      } catch (err) {
        setStatsError('Unable to load profile stats.');
        setPostsError('Unable to load posts.');
      } finally {
        setStatsLoading(false);
      }
    };

    loadStats();
  }, [user]);

  const handleProfileImageChange = (event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) {
      return;
    }

    if (previewIsLocal && preview) {
      URL.revokeObjectURL(preview);
    }

    setRemoveProfilePicture(false);
    setProfileFile(file);
    setPreview(URL.createObjectURL(file));
    setPreviewIsLocal(true);
  };

  const handleRemoveProfileImage = () => {
    if (previewIsLocal && preview) {
      URL.revokeObjectURL(preview);
    }

    setPreview(null);
    setPreviewIsLocal(false);
    setProfileFile(null);
    setRemoveProfilePicture(true);

    if (fileInputRef.current) {
      fileInputRef.current.value = '';
    }
  };

  const onSubmit = async (data: UpdateProfileForm) => {
    try {
      if (!user) return;
      setUploadingImage(!!profileFile);

      let profilePictureUrl = user.profilePicture ?? null;
      if (removeProfilePicture) {
        profilePictureUrl = null;
      } else if (profileFile) {
        const uploaded = await uploadImage(profileFile);
        profilePictureUrl = uploaded.url;
      }

      const updated = await updateUserRequest(user.userId, {
        firstName: data.firstName?.trim() || null,
        lastName: data.lastName?.trim() || null,
        profilePicture: profilePictureUrl,
        bio: data.bio?.trim() || null
      });
      updateUser({
        userId: updated.userId,
        userName: updated.userName,
        email: updated.email,
        firstName: updated.firstName ?? null,
        lastName: updated.lastName ?? null,
        profilePicture: updated.profilePicture ?? null,
        bio: updated.bio ?? null,
        isActive: updated.isActive
      });

      setPreview(updated.profilePicture ?? null);
      setPreviewIsLocal(false);
      setProfileFile(null);
      setRemoveProfilePicture(false);
      if (fileInputRef.current) {
        fileInputRef.current.value = '';
      }
      alert('Profile updated successfully!');
      setIsEditing(false);
    } catch (err) {
      alert('Error updating profile');
    } finally {
      setUploadingImage(false);
    }
  };

  const handleLogout = () => {
    logout();
  };

  return (
    <div className="max-w-2xl mx-auto py-8">
      <div className="bg-white/80 backdrop-blur-xl rounded-2xl shadow-[0_24px_60px_rgba(15,23,42,0.12)] border border-white/60 p-8 dark:bg-slate-900/70 dark:border-slate-800/60">
        {/* Profile Header */}
        <div className="flex items-center gap-6 mb-8 pb-8 border-b border-white/60 dark:border-slate-800/60">
          <div className="w-24 h-24 rounded-full bg-gradient-to-br from-teal-500 via-cyan-500 to-amber-400 flex items-center justify-center flex-shrink-0 shadow-lg shadow-cyan-500/25">
            {previewSrc ? (
              <img src={previewSrc} alt="Avatar" className="w-full h-full rounded-full object-cover" />
            ) : (
              <User className="w-12 h-12 text-white" />
            )}
          </div>
          <div className="flex-1">
            <h1 className="text-3xl font-bold text-slate-900 tracking-tight dark:text-slate-100">
              {[user?.firstName, user?.lastName].filter(Boolean).join(' ') || user?.userName}
            </h1>
            <p className="text-slate-600 mt-2 dark:text-slate-400">{user?.email}</p>
          </div>
          <button
            onClick={handleLogout}
            className="flex items-center gap-2 bg-rose-500 hover:bg-rose-600 text-white px-4 py-2 rounded-xl font-medium shadow-lg shadow-rose-500/20 transition-all"
          >
            <LogOut className="w-4 h-4" />
            Logout
          </button>
        </div>

        {/* Profile Info Section */}
        <div className="mb-8">
          <div className="flex justify-between items-center mb-4">
            <h2 className="text-2xl font-bold text-slate-900 tracking-tight dark:text-slate-100">Profile Information</h2>
            <button
              onClick={() => setIsEditing(!isEditing)}
              className="rounded-xl bg-gradient-to-r from-teal-500 via-cyan-500 to-amber-400 px-4 py-2 text-white font-medium shadow-lg shadow-cyan-500/25 hover:from-teal-600 hover:via-cyan-600 hover:to-amber-500 transition-all"
            >
              {isEditing ? 'Cancel' : 'Edit Profile'}
            </button>
          </div>

          {isEditing ? (
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              <div className="flex flex-col items-center gap-2 mb-6">
                <div className="w-24 h-24 rounded-full bg-white/80 overflow-hidden border-2 border-cyan-300/70 shadow-lg shadow-cyan-500/20 dark:bg-slate-900/60 dark:border-cyan-500/40">
                  {previewSrc ? (
                    <img src={previewSrc} alt="Preview" className="w-full h-full object-cover" />
                  ) : (
                    <div className="flex items-center justify-center h-full text-slate-400 dark:text-slate-500">No Avatar</div>
                  )}
                </div>
              </div>

              <Input
                label="First Name"
                placeholder="Jane"
                {...register('firstName')}
              />

              <Input
                label="Last Name"
                placeholder="Doe"
                {...register('lastName')}
              />

              <div className="space-y-2">
                <label className="text-sm font-medium text-slate-600 dark:text-slate-300">Profile Picture</label>
                <div className="flex flex-wrap items-center gap-3">
                  <label
                    htmlFor={inputId}
                    className="inline-flex items-center gap-2 rounded-xl border border-white/70 bg-white/80 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-white transition-colors cursor-pointer dark:bg-slate-900/70 dark:border-slate-700/60 dark:text-slate-200 dark:hover:bg-slate-800/80"
                  >
                    Choose image
                  </label>
                  <input
                    ref={fileInputRef}
                    id={inputId}
                    type="file"
                    accept="image/*"
                    onChange={handleProfileImageChange}
                    className="hidden"
                  />
                  {profileFile && (
                    <span className="text-xs text-slate-500 dark:text-slate-400">{profileFile.name}</span>
                  )}
                  {preview && (
                    <button
                      type="button"
                      onClick={handleRemoveProfileImage}
                      className="text-xs font-semibold text-rose-500 hover:text-rose-600 dark:text-rose-400 dark:hover:text-rose-300"
                    >
                      Remove
                    </button>
                  )}
                </div>
              </div>

              <div>
                <label className="text-sm font-medium text-slate-600 dark:text-slate-300">Bio</label>
                <textarea
                  {...register('bio')}
                  rows={3}
                  className="mt-1 w-full rounded-xl border border-white/70 bg-white/80 px-3 py-2 text-sm text-slate-700 placeholder:text-slate-400 focus:outline-none focus:ring-2 focus:ring-cyan-300/70 dark:bg-slate-900/70 dark:border-slate-700/60 dark:text-slate-100 dark:placeholder:text-slate-500 dark:focus:ring-cyan-500/40"
                  placeholder="Write a short bio"
                />
              </div>

              <button
                type="submit"
                disabled={isSubmitting || uploadingImage}
                className="w-full rounded-xl bg-emerald-500 hover:bg-emerald-600 text-white font-semibold py-2 shadow-lg shadow-emerald-500/20 transition-all disabled:bg-gray-400 disabled:shadow-none"
              >
                {uploadingImage ? 'Uploading...' : isSubmitting ? 'Saving...' : 'Save Changes'}
              </button>
            </form>
          ) : (
            <div className="space-y-4">
              <div className="bg-white/70 border border-white/60 p-4 rounded-xl dark:bg-slate-900/60 dark:border-slate-800/60">
                <p className="text-sm text-slate-500 dark:text-slate-400">Full Name</p>
                <p className="text-lg font-semibold text-slate-900 dark:text-slate-100">
                  {[user?.firstName, user?.lastName].filter(Boolean).join(' ') || 'Not set'}
                </p>
              </div>
              <div className="bg-white/70 border border-white/60 p-4 rounded-xl dark:bg-slate-900/60 dark:border-slate-800/60">
                <p className="text-sm text-slate-500 dark:text-slate-400">Username</p>
                <p className="text-lg font-semibold text-slate-900 dark:text-slate-100">{user?.userName}</p>
              </div>
              <div className="bg-white/70 border border-white/60 p-4 rounded-xl dark:bg-slate-900/60 dark:border-slate-800/60">
                <p className="text-sm text-slate-500 dark:text-slate-400">Email</p>
                <p className="text-lg font-semibold text-slate-900 dark:text-slate-100">{user?.email}</p>
              </div>
              <div className="bg-white/70 border border-white/60 p-4 rounded-xl dark:bg-slate-900/60 dark:border-slate-800/60">
                <p className="text-sm text-slate-500 dark:text-slate-400">Bio</p>
                <p className="text-lg font-semibold text-slate-900 dark:text-slate-100">{user?.bio || 'No bio yet'}</p>
              </div>
            </div>
          )}
        </div>

        {/* Account Stats */}
        <div className="grid grid-cols-3 gap-4">
          <div className="bg-teal-50/80 border border-teal-100/70 p-4 rounded-xl text-center dark:bg-teal-500/10 dark:border-teal-500/20">
            <p className="text-2xl font-bold text-teal-600">
              {statsLoading ? '—' : postsCount ?? 0}
            </p>
            <p className="text-sm text-slate-600 dark:text-slate-400">Posts</p>
          </div>
          <div className="bg-amber-50/80 border border-amber-100/70 p-4 rounded-xl text-center dark:bg-amber-500/10 dark:border-amber-500/20">
            <p className="text-2xl font-bold text-amber-600">
              {statsLoading ? '—' : friendsCount ?? 0}
            </p>
            <p className="text-sm text-slate-600 dark:text-slate-400">Friends</p>
          </div>
          <div className="bg-rose-50/80 border border-rose-100/70 p-4 rounded-xl text-center dark:bg-rose-500/10 dark:border-rose-500/20">
            <p className="text-2xl font-bold text-rose-600">
              {statsLoading ? '—' : pendingCount ?? 0}
            </p>
            <p className="text-sm text-slate-600 dark:text-slate-400">Pending</p>
          </div>
        </div>
        {statsError && (
          <div className="mt-3 text-sm text-rose-600 dark:text-rose-400">
            {statsError}
          </div>
        )}

        <div className="mt-8 space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-2xl font-bold text-slate-900 tracking-tight dark:text-slate-100">Your Posts</h2>
          </div>

          {postsError && (
            <div className="rounded-xl border border-rose-200/70 bg-rose-50/70 px-4 py-3 text-sm text-rose-700 dark:border-rose-500/30 dark:bg-rose-500/10 dark:text-rose-300">
              {postsError}
            </div>
          )}

          {statsLoading ? (
            <div className="space-y-4">
              <PostSkeleton />
              <PostSkeleton />
            </div>
          ) : posts.length === 0 ? (
            <div className="rounded-2xl border border-white/60 bg-white/70 px-4 py-6 text-center text-slate-600 backdrop-blur-xl dark:border-slate-800/60 dark:bg-slate-900/60 dark:text-slate-300">
              No posts yet.
            </div>
          ) : (
            <div className="space-y-4">
              {posts.map((post) => (
                <PostCard key={post.postId} post={post} showCommentsByDefault />
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default Profile;
