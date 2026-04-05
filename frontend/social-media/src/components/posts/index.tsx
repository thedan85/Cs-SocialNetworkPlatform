import { useState, type KeyboardEvent } from 'react'
import { Link } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { Avatar } from '@/components/common/Avatar'
import { Button } from '@/components/common/Button'
import { Skeleton, SkeletonText } from '@/components/common/index'
import { TextInput, FileInput } from '@/components/forms/index'
import { useAuth } from '@/context/AuthContext'
import postService from '@/services/postService'
import { timeAgo, formatCount, createFilePreview } from '@/utils/helpers'
import type { Post, PostFormData } from '@/types'

// ─── PostCard ─────────────────────────────────────────────────────────────────
interface PostCardProps {
  post:        Post
  onLike:      (id: string) => void
  onSave:      (id: string) => void
  onDelete?:   (id: string) => void
  currentUserId?: string
}

export function PostCard({ post, onLike, onSave, onDelete, currentUserId }: PostCardProps) {
  const [showMenu, setShowMenu] = useState(false)

  const isOwner = currentUserId === post.author.id

  return (
    <article className="card p-4 animate-fade-in">
      {/* Header */}
      <div className="flex items-start justify-between mb-3">
        <div className="flex items-center gap-3">
          <Link to={`/profile/${post.author.username}`}>
            <Avatar
              src={post.author.avatarUrl}
              username={post.author.username}
              fullName={post.author.fullName}
              size="md"
            />
          </Link>
          <div>
            <Link
              to={`/profile/${post.author.username}`}
              className="font-semibold text-slate-900 hover:text-primary-600 transition-colors text-sm"
            >
              {post.author.fullName}
            </Link>
            <p className="text-xs text-slate-500">
              @{post.author.username} · {timeAgo(post.createdAt)}
            </p>
          </div>
        </div>
        {isOwner && (
          <div className="relative">
            <button
              onClick={() => setShowMenu(v => !v)}
              className="p-1.5 rounded-lg hover:bg-slate-100 text-slate-400 transition-colors"
            >
              <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 24 24">
                <circle cx="12" cy="5"  r="1.5" /><circle cx="12" cy="12" r="1.5" /><circle cx="12" cy="19" r="1.5" />
              </svg>
            </button>
            {showMenu && (
              <div className="absolute right-0 mt-1 w-40 card shadow-xl z-10 py-1 animate-fade-in">
                <button
                  onClick={() => { onDelete?.(post.id); setShowMenu(false) }}
                  className="w-full text-left px-4 py-2 text-sm text-red-500 hover:bg-red-50 transition-colors"
                >
                  Delete post
                </button>
              </div>
            )}
          </div>
        )}
      </div>

      {/* Content */}
      <p className="text-slate-800 text-sm leading-relaxed mb-3 whitespace-pre-wrap">{post.content}</p>

      {post.imageUrl && (
        <img
          src={post.imageUrl}
          alt="Post image"
          loading="lazy"
          className="w-full rounded-xl object-cover max-h-96 mb-3 bg-slate-100"
        />
      )}

      {/* Actions */}
      <div className="flex items-center gap-1 pt-2 border-t border-slate-100">
        <ActionButton
          icon={<HeartIcon filled={post.isLiked} />}
          count={post.likesCount}
          active={post.isLiked}
          activeColor="text-red-500"
          onClick={() => onLike(post.id)}
          label="Like"
        />
        <ActionButton
          icon={<CommentIcon />}
          count={post.commentsCount}
          label="Comment"
        />
        <ActionButton
          icon={<ShareIcon />}
          count={post.sharesCount}
          label="Share"
        />
        <div className="ml-auto">
          <ActionButton
            icon={<BookmarkIcon filled={post.isSaved} />}
            active={post.isSaved}
            activeColor="text-primary-600"
            onClick={() => onSave(post.id)}
            label="Save"
          />
        </div>
      </div>
    </article>
  )
}

function ActionButton({
  icon, count, active, activeColor, onClick, label,
}: {
  icon: React.ReactNode; count?: number; active?: boolean; activeColor?: string; onClick?: () => void; label: string
}) {
  return (
    <button
      onClick={onClick}
      aria-label={label}
      className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-medium
        transition-all duration-150 hover:bg-slate-100
        ${active ? activeColor : 'text-slate-500'}`}
    >
      {icon}
      {count !== undefined && <span>{formatCount(count)}</span>}
    </button>
  )
}

// ─── Icons ─────────────────────────────────────────────────────────────────────
const HeartIcon = ({ filled }: { filled?: boolean }) => (
  <svg className="w-4 h-4" fill={filled ? 'currentColor' : 'none'} viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
    <path strokeLinecap="round" strokeLinejoin="round" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
  </svg>
)

const CommentIcon = () => (
  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
    <path strokeLinecap="round" strokeLinejoin="round" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
  </svg>
)

const ShareIcon = () => (
  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
    <path strokeLinecap="round" strokeLinejoin="round" d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z" />
  </svg>
)

const BookmarkIcon = ({ filled }: { filled?: boolean }) => (
  <svg className="w-4 h-4" fill={filled ? 'currentColor' : 'none'} viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
    <path strokeLinecap="round" strokeLinejoin="round" d="M5 5a2 2 0 012-2h10a2 2 0 012 2v16l-7-3.5L5 21V5z" />
  </svg>
)

// ─── PostSkeleton ─────────────────────────────────────────────────────────────
export function PostSkeleton() {
  return (
    <div className="card p-4">
      <div className="flex items-center gap-3 mb-3">
        <Skeleton className="h-10 w-10 rounded-full" />
        <div className="flex-1 space-y-1.5">
          <Skeleton className="h-3.5 w-32" />
          <Skeleton className="h-3 w-24" />
        </div>
      </div>
      <SkeletonText lines={3} />
      <Skeleton className="h-48 w-full mt-3" />
    </div>
  )
}

// ─── PostForm ─────────────────────────────────────────────────────────────────
interface PostFormProps {
  onSuccess?: (post: Post) => void
}

export function PostForm({ onSuccess }: PostFormProps) {
  const { user } = useAuth()
  const [preview, setPreview] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(false)

  const { register, handleSubmit, reset, watch, formState: { errors } } = useForm<PostFormData>()
  const content = watch('content', '')

  const handleImageChange = async (files: FileList | null) => {
    if (!files?.[0]) return
    const url = await createFilePreview(files[0])
    setPreview(url)
  }

  const onSubmit = async (data: PostFormData) => {
    setIsLoading(true)
    try {
      const post = await postService.createPost(data)
      onSuccess?.(post)
      reset()
      setPreview(null)
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <div className="card p-4">
      <div className="flex gap-3">
        <Avatar src={user?.avatarUrl} username={user?.username} fullName={user?.fullName} size="md" />
        <div className="flex-1">
          <textarea
            {...register('content', { required: "What's on your mind?" })}
            placeholder="What's on your mind?"
            rows={3}
            className="w-full resize-none border-0 bg-transparent text-slate-800 placeholder-slate-400
                       focus:outline-none text-sm leading-relaxed"
          />
          {errors.content && (
            <p className="text-xs text-red-500 mb-2">{errors.content.message}</p>
          )}

          <FileInput
            onChange={handleImageChange}
            preview={preview}
            accept=".jpg,.jpeg,.png,.webp,.gif"
            helpText="JPG, PNG, WebP, GIF · max 10MB"
          />

          <div className="flex items-center justify-between mt-3 pt-3 border-t border-slate-100">
            <span className={`text-xs ${content.length > 280 ? 'text-red-500' : 'text-slate-400'}`}>
              {content.length}/280
            </span>
            <Button
              onClick={handleSubmit(onSubmit)}
              isLoading={isLoading}
              disabled={!content.trim() || content.length > 280}
              size="sm"
            >
              Post
            </Button>
          </div>
        </div>
      </div>
    </div>
  )
}

// ─── PostList ─────────────────────────────────────────────────────────────────
import { useInfiniteScroll } from '@/hooks/useUtils'

interface PostListProps {
  posts:     Post[]
  isLoading: boolean
  hasMore:   boolean
  onLoadMore: () => void
  onLike:    (id: string) => void
  onSave:    (id: string) => void
  onDelete?: (id: string) => void
  currentUserId?: string
  emptyMessage?: string
}

export function PostList({
  posts, isLoading, hasMore, onLoadMore, onLike, onSave, onDelete, currentUserId,
  emptyMessage = 'No posts yet.',
}: PostListProps) {
  const sentinelRef = useInfiniteScroll(onLoadMore, hasMore, isLoading)

  if (!isLoading && posts.length === 0) {
    return (
      <div className="card p-12 text-center">
        <p className="text-slate-500 text-sm">{emptyMessage}</p>
      </div>
    )
  }

  return (
    <div className="space-y-4">
      {posts.map(post => (
        <PostCard
          key={post.id}
          post={post}
          onLike={onLike}
          onSave={onSave}
          onDelete={onDelete}
          currentUserId={currentUserId}
        />
      ))}

      {isLoading && Array.from({ length: 3 }).map((_, i) => <PostSkeleton key={i} />)}

      {/* Infinite scroll sentinel */}
      <div ref={sentinelRef} className="h-4" />
    </div>
  )
}
