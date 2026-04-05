import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams, useSearchParams } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { useAuth } from '@/context/AuthContext'
import { useNotifications } from '@/context/NotificationContext'
import { usePosts } from '@/hooks/usePosts'
import { useDebounce } from '@/hooks/useUtils'
import { PostList, PostForm } from '@/components/posts/index'
import { UserCard, UserCardSkeleton } from '@/components/users/index'
import { Avatar } from '@/components/common/Avatar'
import { Button } from '@/components/common/Button'
import { Spinner } from '@/components/common/Spinner'
import { Skeleton, SkeletonText, Modal } from '@/components/common/index'
import { FriendButton } from '@/components/users/index'
import { TextInput } from '@/components/forms/index'
import { NotificationItem } from '@/components/notifications/index'
import userService from '@/services/userService'
import { formatCount } from '@/utils/helpers'
import type { ProfileUpdateFormData, User } from '@/types'

// ─── HomePage ─────────────────────────────────────────────────────────────────
export function HomePage() {
  const { user } = useAuth()
  const {
    posts, isLoading, hasMore, loadMore,
    toggleLike, toggleSave, removePost, addPost,
  } = usePosts()

  return (
    <div className="max-w-2xl mx-auto space-y-4">
      <PostForm onSuccess={addPost} />
      <PostList
        posts={posts}
        isLoading={isLoading}
        hasMore={hasMore}
        onLoadMore={loadMore}
        onLike={toggleLike}
        onSave={toggleSave}
        onDelete={removePost}
        currentUserId={user?.id}
        emptyMessage="Your feed is empty. Follow some people to see their posts!"
      />
    </div>
  )
}

// ─── SearchPage ───────────────────────────────────────────────────────────────
export function SearchPage() {
  const [searchParams, setSearchParams] = useSearchParams()
  const [query,     setQuery]     = useState(searchParams.get('q') ?? '')
  const [tab,       setTab]       = useState<'users' | 'posts'>('users')
  const [users,     setUsers]     = useState<User[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const debouncedQuery = useDebounce(query, 400)

  useEffect(() => {
    if (!debouncedQuery.trim()) { setUsers([]); return }
    setSearchParams({ q: debouncedQuery })
    const run = async () => {
      setIsLoading(true)
      try {
        const res = await userService.searchUsers(debouncedQuery)
        setUsers(res.items)
      } finally {
        setIsLoading(false)
      }
    }
    run()
  }, [debouncedQuery, setSearchParams])

  return (
    <div className="max-w-2xl mx-auto space-y-4">
      <div className="card p-4">
        <div className="relative">
          <svg className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400"
            fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          <input
            autoFocus
            type="search"
            value={query}
            onChange={e => setQuery(e.target.value)}
            placeholder="Search people and posts…"
            className="input-field pl-10 text-base"
          />
        </div>
      </div>

      <div className="flex gap-1 card p-1">
        {(['users', 'posts'] as const).map(t => (
          <button
            key={t}
            onClick={() => setTab(t)}
            className={`flex-1 py-2 rounded-xl text-sm font-medium capitalize transition-all duration-150
              ${tab === t ? 'bg-primary-600 text-white shadow-sm' : 'text-slate-600 hover:bg-slate-100'}`}
          >
            {t}
          </button>
        ))}
      </div>

      {!debouncedQuery.trim() && (
        <div className="card p-16 text-center">
          <svg className="w-10 h-10 text-slate-300 mx-auto mb-3" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
          <p className="text-slate-500 text-sm">Start typing to search…</p>
        </div>
      )}

      {tab === 'users' && debouncedQuery && (
        <div className="space-y-3">
          {isLoading && Array.from({ length: 4 }).map((_, i) => <UserCardSkeleton key={i} />)}
          {!isLoading && users.length === 0 && (
            <div className="card p-12 text-center text-slate-500 text-sm">
              No users found for "{debouncedQuery}"
            </div>
          )}
          {users.map(u => <UserCard key={u.id} user={u} />)}
        </div>
      )}

      {tab === 'posts' && debouncedQuery && <PostSearchResults />}
    </div>
  )
}

function PostSearchResults() {
  const { posts, isLoading, hasMore, loadMore, toggleLike, toggleSave } = usePosts()
  return (
    <PostList
      posts={posts}
      isLoading={isLoading}
      hasMore={hasMore}
      onLoadMore={loadMore}
      onLike={toggleLike}
      onSave={toggleSave}
      emptyMessage="No posts found"
    />
  )
}

// ─── ProfilePage ──────────────────────────────────────────────────────────────
export function ProfilePage() {
  const { username }    = useParams<{ username: string }>()
  const { user: me, updateUser } = useAuth()
  const [profile,       setProfile]       = useState<User | null>(null)
  const [isLoadingProf, setIsLoadingProf] = useState(true)
  const [showEdit,      setShowEdit]      = useState(false)

  const {
    posts, isLoading, hasMore, loadMore,
    toggleLike, toggleSave, removePost,
  } = usePosts({ username })

  useEffect(() => {
    if (!username) return
    setIsLoadingProf(true)
    userService.getProfile(username)
      .then(setProfile)
      .finally(() => setIsLoadingProf(false))
  }, [username])

  const isOwner = me?.username === username

  return (
    <div className="max-w-2xl mx-auto space-y-4">
      <div className="card overflow-hidden">
        {/* Cover */}
        <div className="h-44 bg-gradient-to-br from-primary-400 via-indigo-500 to-purple-600">
          {profile?.coverUrl && (
            <img src={profile.coverUrl} alt="Cover" className="w-full h-full object-cover" />
          )}
        </div>

        <div className="px-5 pb-5">
          <div className="flex items-end justify-between -mt-10 mb-4">
            {isLoadingProf ? (
              <Skeleton className="h-20 w-20 rounded-full ring-4 ring-white" />
            ) : (
              <Avatar
                src={profile?.avatarUrl}
                username={profile?.username}
                fullName={profile?.fullName}
                size="xl"
                className="ring-4 ring-white shadow-lg"
              />
            )}
            <div className="flex gap-2 pb-1">
              {isOwner ? (
                <Button variant="outline" size="sm" onClick={() => setShowEdit(true)}>
                  Edit profile
                </Button>
              ) : profile ? (
                <>
                  <Button variant="outline" size="sm">Message</Button>
                  <FriendButton
                    userId={profile.id}
                    friendStatus={profile.isFriend ? 'friends' : 'none'}
                  />
                </>
              ) : null}
            </div>
          </div>

          {isLoadingProf ? (
            <div className="space-y-2">
              <Skeleton className="h-5 w-40" />
              <Skeleton className="h-4 w-28" />
              <SkeletonText lines={2} />
            </div>
          ) : profile && (
            <>
              <h1 className="text-xl font-bold text-slate-900">{profile.fullName}</h1>
              <p className="text-sm text-slate-500 mb-2">@{profile.username}</p>
              {profile.bio && (
                <p className="text-slate-700 text-sm leading-relaxed mb-3">{profile.bio}</p>
              )}
              <div className="flex gap-5 text-sm">
                <span className="text-slate-600">
                  <strong className="font-semibold text-slate-900">{formatCount(profile.postsCount)}</strong> posts
                </span>
                <button className="text-slate-600 hover:text-primary-600 transition-colors">
                  <strong className="font-semibold text-slate-900">{formatCount(profile.followersCount)}</strong> followers
                </button>
                <button className="text-slate-600 hover:text-primary-600 transition-colors">
                  <strong className="font-semibold text-slate-900">{formatCount(profile.followingCount)}</strong> following
                </button>
              </div>
            </>
          )}
        </div>
      </div>

      <PostList
        posts={posts}
        isLoading={isLoading}
        hasMore={hasMore}
        onLoadMore={loadMore}
        onLike={toggleLike}
        onSave={toggleSave}
        onDelete={isOwner ? removePost : undefined}
        currentUserId={me?.id}
        emptyMessage={isOwner ? "You haven't posted anything yet." : 'No posts yet.'}
      />

      {me && (
        <Modal isOpen={showEdit} onClose={() => setShowEdit(false)} title="Edit profile" size="md">
          <EditProfileForm
            user={me}
            onSuccess={updated => {
              setProfile(updated)
              updateUser(updated)
              setShowEdit(false)
            }}
          />
        </Modal>
      )}
    </div>
  )
}

// ─── EditProfileForm ──────────────────────────────────────────────────────────
interface EditProfileFormProps {
  user:      User
  onSuccess: (updated: User) => void
}

function EditProfileForm({ user, onSuccess }: EditProfileFormProps) {
  const {
    register, handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<ProfileUpdateFormData>({
    defaultValues: { fullName: user.fullName, bio: user.bio ?? '' },
  })

  const onSubmit = async (data: ProfileUpdateFormData) => {
    const updated = await userService.updateProfile(data)
    onSuccess(updated)
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4" noValidate>
      <TextInput
        label="Full name"
        error={errors.fullName?.message}
        {...register('fullName', {
          required:  'Full name is required',
          minLength: { value: 2, message: 'At least 2 characters' },
        })}
      />
      <div>
        <label className="block text-sm font-medium text-slate-700 mb-1.5">Bio</label>
        <textarea
          rows={3}
          placeholder="Tell people about yourself…"
          className="input-field resize-none"
          {...register('bio', {
            maxLength: { value: 160, message: 'Max 160 characters' },
          })}
        />
        {errors.bio && <p className="mt-1 text-xs text-red-500">{errors.bio.message}</p>}
      </div>
      <Button type="submit" fullWidth isLoading={isSubmitting}>Save changes</Button>
    </form>
  )
}

// ─── NotificationsPage ────────────────────────────────────────────────────────
export function NotificationsPage() {
  const { notifications, unreadCount, markAllAsRead, markAsRead, loadMore, hasMore } = useNotifications()
  const [isLoadingMore, setIsLoadingMore] = useState(false)

  const handleLoadMore = async () => {
    setIsLoadingMore(true)
    await loadMore()
    setIsLoadingMore(false)
  }

  return (
    <div className="max-w-2xl mx-auto">
      <div className="card overflow-hidden">
        <div className="flex items-center justify-between px-5 py-4 border-b border-slate-100">
          <div>
            <h1 className="font-bold text-slate-900 text-lg">Notifications</h1>
            {unreadCount > 0 && (
              <p className="text-xs text-slate-500">{unreadCount} unread</p>
            )}
          </div>
          {unreadCount > 0 && (
            <Button variant="ghost" size="sm" onClick={markAllAsRead}>Mark all read</Button>
          )}
        </div>

        {notifications.length === 0 ? (
          <div className="py-20 text-center">
            <svg className="w-10 h-10 text-slate-200 mx-auto mb-3" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={1.5}>
              <path strokeLinecap="round" strokeLinejoin="round"
                d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
            </svg>
            <p className="text-slate-500 text-sm">You're all caught up!</p>
          </div>
        ) : (
          <div className="divide-y divide-slate-50">
            {notifications.map(n => (
              <NotificationItem key={n.id} notification={n} onRead={markAsRead} />
            ))}
          </div>
        )}

        {hasMore && (
          <div className="px-5 py-4 border-t border-slate-100 text-center">
            <Button variant="ghost" size="sm" onClick={handleLoadMore} isLoading={isLoadingMore}>
              Load more
            </Button>
          </div>
        )}
      </div>
    </div>
  )
}

// ─── SettingsPage ─────────────────────────────────────────────────────────────
export function SettingsPage() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = async () => {
    await logout()
    navigate('/login', { replace: true })
  }

  return (
    <div className="max-w-2xl mx-auto space-y-4">
      <h1 className="text-2xl font-bold text-slate-900">Settings</h1>

      <div className="card divide-y divide-slate-100">
        <div className="px-5 py-4">
          <h2 className="font-semibold text-slate-900 mb-1">Account</h2>
          <p className="text-sm text-slate-500">Manage your account details</p>
        </div>
        <div className="px-5 py-4 flex items-center gap-4">
          <Avatar src={user?.avatarUrl} username={user?.username} fullName={user?.fullName} size="lg" />
          <div>
            <p className="font-semibold text-slate-900">{user?.fullName}</p>
            <p className="text-sm text-slate-500">@{user?.username}</p>
            <p className="text-sm text-slate-500">{user?.email}</p>
          </div>
        </div>
        <div className="px-5 py-4">
          <Link to={`/profile/${user?.username}`}>
            <Button variant="outline" size="sm">View profile</Button>
          </Link>
        </div>
      </div>

      <div className="card divide-y divide-slate-100">
        <div className="px-5 py-4">
          <h2 className="font-semibold text-red-600 mb-1">Danger zone</h2>
        </div>
        <div className="px-5 py-4 flex items-center justify-between">
          <div>
            <p className="text-sm font-medium text-slate-900">Sign out</p>
            <p className="text-xs text-slate-500">Sign out of your account</p>
          </div>
          <Button variant="danger" size="sm" onClick={handleLogout}>Sign out</Button>
        </div>
      </div>
    </div>
  )
}

// ─── NotFoundPage ─────────────────────────────────────────────────────────────
export function NotFoundPage() {
  return (
    <div className="min-h-screen flex items-center justify-center bg-slate-50 p-4">
      <div className="text-center">
        <p className="text-8xl font-bold text-primary-600 mb-4 leading-none">404</p>
        <h1 className="text-2xl font-bold text-slate-900 mb-2">Page not found</h1>
        <p className="text-slate-500 mb-8 text-sm">The page you're looking for doesn't exist.</p>
        <Link to="/"><Button>Back to home</Button></Link>
      </div>
    </div>
  )
}
