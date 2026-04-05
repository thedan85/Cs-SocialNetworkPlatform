import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Avatar } from '@/components/common/Avatar'
import { Button } from '@/components/common/Button'
import { Skeleton } from '@/components/common/index'
import userService from '@/services/userService'
import { formatCount, truncate } from '@/utils/helpers'
import type { FriendStatus, User } from '@/types'

// ─── UserCard ─────────────────────────────────────────────────────────────────
interface UserCardProps {
  user:      User
  compact?:  boolean
  onFollow?: (userId: string, isFollowing: boolean) => void
}

export function UserCard({ user, compact = false, onFollow }: UserCardProps) {
  const [isFollowing, setIsFollowing] = useState(user.isFollowing ?? false)
  const [isLoading, setIsLoading] = useState(false)

  const handleFollow = async () => {
    setIsLoading(true)
    try {
      if (isFollowing) {
        await userService.unfollow(user.id)
      } else {
        await userService.follow(user.id)
      }
      setIsFollowing(v => !v)
      onFollow?.(user.id, !isFollowing)
    } finally {
      setIsLoading(false)
    }
  }

  if (compact) {
    return (
      <div className="flex items-center gap-3">
        <Link to={`/profile/${user.username}`}>
          <Avatar src={user.avatarUrl} username={user.username} fullName={user.fullName} size="sm" />
        </Link>
        <div className="flex-1 min-w-0">
          <Link
            to={`/profile/${user.username}`}
            className="block text-sm font-semibold text-slate-900 hover:text-primary-600 transition-colors truncate"
          >
            {user.fullName}
          </Link>
          <p className="text-xs text-slate-500 truncate">@{user.username}</p>
        </div>
        <Button variant="outline" size="sm" onClick={handleFollow} isLoading={isLoading}>
          {isFollowing ? 'Unfollow' : 'Follow'}
        </Button>
      </div>
    )
  }

  return (
    <article className="card p-5 hover:shadow-md transition-shadow">
      <div className="flex items-start gap-4">
        <Link to={`/profile/${user.username}`}>
          <Avatar src={user.avatarUrl} username={user.username} fullName={user.fullName} size="lg" />
        </Link>
        <div className="flex-1 min-w-0">
          <Link
            to={`/profile/${user.username}`}
            className="block font-semibold text-slate-900 hover:text-primary-600 transition-colors"
          >
            {user.fullName}
          </Link>
          <p className="text-sm text-slate-500 mb-1">@{user.username}</p>
          {user.bio && (
            <p className="text-sm text-slate-600 mb-3">{truncate(user.bio, 100)}</p>
          )}
          <div className="flex gap-4 text-xs text-slate-500 mb-3">
            <span><strong className="text-slate-800">{formatCount(user.followersCount)}</strong> followers</span>
            <span><strong className="text-slate-800">{formatCount(user.followingCount)}</strong> following</span>
          </div>
          <Button
            variant={isFollowing ? 'outline' : 'primary'}
            size="sm"
            onClick={handleFollow}
            isLoading={isLoading}
          >
            {isFollowing ? 'Unfollow' : 'Follow'}
          </Button>
        </div>
      </div>
    </article>
  )
}

// ─── UserCardSkeleton ─────────────────────────────────────────────────────────
export function UserCardSkeleton() {
  return (
    <div className="card p-5">
      <div className="flex items-start gap-4">
        <Skeleton className="h-14 w-14 rounded-full shrink-0" />
        <div className="flex-1 space-y-2">
          <Skeleton className="h-4 w-32" />
          <Skeleton className="h-3 w-24" />
          <Skeleton className="h-3 w-full" />
          <Skeleton className="h-3 w-3/4" />
        </div>
      </div>
    </div>
  )
}

// ─── FriendButton ─────────────────────────────────────────────────────────────
interface FriendButtonProps {
  userId:       string
  friendStatus: FriendStatus
  requestId?:   string
  onStatusChange?: (newStatus: FriendStatus) => void
}

export function FriendButton({ userId, friendStatus, requestId, onStatusChange }: FriendButtonProps) {
  const [status,    setStatus]    = useState(friendStatus)
  const [isLoading, setIsLoading] = useState(false)

  const handleClick = async () => {
    setIsLoading(true)
    try {
      if (status === 'none') {
        await userService.sendFriendRequest(userId)
        setStatus('pending')
        onStatusChange?.('pending')
      } else if (status === 'pending' && requestId) {
        await userService.rejectFriendRequest(requestId)
        setStatus('none')
        onStatusChange?.('none')
      }
    } finally {
      setIsLoading(false)
    }
  }

  const config: Record<FriendStatus, { label: string; variant: 'primary' | 'outline' | 'secondary' }> = {
    none:    { label: 'Add friend',   variant: 'primary' },
    pending: { label: 'Pending…',     variant: 'secondary' },
    friends: { label: '✓ Friends',    variant: 'outline' },
    blocked: { label: 'Blocked',      variant: 'secondary' },
  }

  const { label, variant } = config[status]

  return (
    <Button variant={variant} size="sm" onClick={handleClick} isLoading={isLoading}>
      {label}
    </Button>
  )
}
